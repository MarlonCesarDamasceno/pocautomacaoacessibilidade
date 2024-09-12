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

        public static string ExtrairNomeDominiio(string url)
        {
            string pattern= @"(?:https?:\/\/)?(?:www\.)?([^\.]+)\.";
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
