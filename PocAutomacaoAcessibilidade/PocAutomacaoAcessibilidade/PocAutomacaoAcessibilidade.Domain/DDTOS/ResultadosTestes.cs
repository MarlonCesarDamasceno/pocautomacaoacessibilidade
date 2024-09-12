using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS
{
public     class ResultadosTestes
    {
        public List<ResultadoValidacao> resultadosValidacoes { get; set; }
        public List<AnalisePreviaResultadoTeste> analisesPreliminaresValidacoes { get; set; }
    }
}
