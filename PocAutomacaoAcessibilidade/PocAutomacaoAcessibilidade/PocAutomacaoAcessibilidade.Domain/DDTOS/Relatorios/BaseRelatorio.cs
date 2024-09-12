using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS.Relatorios
{
    public class BaseRelatorio
    {
        public int PaginaRelatorio { get; set; }
        public string ServicoTestado { get; set; }
        public string Impacto { get; set; }
        public string Descricao { get; set; }

        
        public List<BaseRelatorioComponente> baseRelatorioComponentes { get; set; }
    }
}
