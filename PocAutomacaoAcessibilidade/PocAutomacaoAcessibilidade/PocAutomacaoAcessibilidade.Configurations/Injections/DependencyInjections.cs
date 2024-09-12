using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Aplication.Services;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces.Services;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Infra;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Configurations
{
    public static class DependencyInjections
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public static void ConfigureServices(this IServiceCollection services)
        {
            AllocConsole();
            services.AddTransient<IMotorAcessibilidade, MotorAcessibilidade>();
            services.AddTransient<IMotorAcessibilidadeService, MotorAcessibilidadeService>();
            services.AddTransient<IRelatorioService, RelatorioService>();
            services.AddTransient<M6Acessibilidade>();

            services.AddLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            });
                

        }
    }
}
