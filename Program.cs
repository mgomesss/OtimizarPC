using System;
using Microsoft.Win32;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        try
        {
            ModoDesempenhoMaximo();
            LimparPastasSeguras();
            DesativarAplicativosEmSegundoPlano();
            DesativarEfeitosDeTransparencia();
            OtimizarHD("C");
            AtualizarPC();
            //EscanearPorVirus();
            AguardarParaFechar();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro: " + ex.Message);
        }
    }

    static void ModoDesempenhoMaximo()
    {
        // Obter o esquema de energia atualmente ativo
        string activeScheme = ExecuteCommand("/C powercfg /getactivescheme");

        // Extrair o GUID do esquema ativo a partir da saída
        string activeGuid = activeScheme.Substring(activeScheme.IndexOf(":") + 2, 36);

        // Verificar se o esquema de Alto Desempenho já está ativo
        if (activeGuid.Equals("8e06e0da-71ae-4758-98fb-8ea8bc203548", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("O esquema de Desempenho Maximo já está ativo.");
        }
        else
        {
            string output = ExecuteCommand("/C powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
            string guid = output.Substring(output.IndexOf(":") + 2, 36);
            ExecuteCommand("/C powercfg -setactive " + guid);
            Console.WriteLine("Esquema de Desempenho Maximo ativado.");
        }
    }
    static void LimparPastasSeguras()
    {
        // Limpar a pasta Temp
        ExecuteCommand("/C del /s /q %temp%\\*");
        Console.WriteLine("Pasta temp limpada.");


        // Esvaziar a Lixeira
        ExecuteCommand("/C PowerShell Clear-RecycleBin -Force -ErrorAction:Ignore");
        Console.WriteLine("Lixeira esvaziada");

    }
    static void DesativarAplicativosEmSegundoPlano()
    {
        const string keyName = @"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications";
        const string valueName = "GlobalUserDisabled";
        const int disableValue = 1;

        try
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(keyName);
            }

            key.SetValue(valueName, disableValue, RegistryValueKind.DWord);
            key.Close();

            Console.WriteLine("Aplicativos em segundo plano foram desativados.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao desativar aplicativos em segundo plano: {ex.Message}");
        }
    }
    static void DesativarEfeitosDeTransparencia()
    {
        const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        try
        {
            Registry.SetValue(keyName, "EnableTransparency", 0, RegistryValueKind.DWord);
            Console.WriteLine("Efeitos de transparência foram desativados.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao desativar efeitos de transparência: {ex.Message}");
        }
    }
    static void AguardarParaFechar()
    {
        Console.ReadLine();
    }
    static void OtimizarHD(string driveLetter)
    {
        // Certifique-se de que a letra da unidade seja válida.
        if (!string.IsNullOrEmpty(driveLetter) && driveLetter.Length == 1)
        {
            string output = ExecuteCommand($"/C defrag {driveLetter}: /O /U");
            Console.WriteLine("HD Otimizado");
        }
        else
        {
            Console.WriteLine("Letra de unidade inválida.");
        }
    }
    static void AtualizarPC()
    {
        string output = ExecuteCommand("/C wuauclt.exe /updatenow");
        Console.WriteLine("Drivers sendo atualizados");
    }
    static void EscanearPorVirus()
    {
        string output = ExecuteCommand("/C mrt.exe /F:Y");
        Console.WriteLine("Escaneamento por virus concluido");

    }


    static string ExecuteCommand(string command)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = command;

        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        //Console.WriteLine(output);

        process.WaitForExit();

        return output;
    }
}
