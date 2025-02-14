﻿using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces
{
    public interface IMotorAcessibilidade
    {
        string ObterAplicabilidadeDeCodigo(string id);
        string ObterDescricaoErro(string id);
        Task<List<ResultadoValidacao>> ValidarAcessibilidade(string Url);
    }
}
