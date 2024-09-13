using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Utils
{
    public static class Utils
    {
        public static void RegistrarServicoTestado(string url)
        {
            ParametrosSistema.ServicoTestado = url;
        }

        public static string ObterServicoTesteRegistrado()
        {
            return ParametrosSistema.ServicoTestado;
        }

        public static bool ValidarAlteracaoAlvoTeste(string url)
        {
            if (url != ParametrosSistema.ServicoTestado)
            {
                ParametrosSistema.QuantidadeServicosTestados = 0;
                return true;
            }

            return default;
        }

        public static void ControlarServicoTestado()
        {
            ParametrosSistema.QuantidadeServicosTestados++;
        }

        public static int ObterControleServicoTestado()
        {
            return ParametrosSistema.QuantidadeServicosTestados;
        }


        public static string ExtrairNomeDominiio(string url)
        {
            string pattern = @"(?:https?:\/\/)?(?:www\.)?([^\.]+)\.";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(url);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}
