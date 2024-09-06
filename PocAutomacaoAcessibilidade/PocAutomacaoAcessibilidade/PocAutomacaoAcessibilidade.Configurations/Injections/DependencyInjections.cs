﻿using Microsoft.Extensions.DependencyInjection;
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

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IMotorAcessibilidade, MotorAcessibilidade>();
            services.AddTransient<IMotorAcessibilidadeService, MotorAcessibilidadeService>();
            services.AddTransient<IRelatorioService, RelatorioService>();
            services.AddTransient<M6Acessibilidade>();

        }
    }
}
