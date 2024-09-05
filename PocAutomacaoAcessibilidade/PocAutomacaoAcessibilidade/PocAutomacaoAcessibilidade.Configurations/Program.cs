using Microsoft.Extensions.DependencyInjection;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Configurations;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PocAutomacaoAcessibilidade
{
    static class Program
    {
        public static IServiceProvider ServiceProvider;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            DependencyInjections.ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var m6AcessibilidadeForm = ServiceProvider.GetService<M6Acessibilidade>();


            Application.Run(m6AcessibilidadeForm);
        }
    }
}
