using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Mappers;
using Selenium.Axe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Infra
{
    public class MotorAcessibilidade : IMotorAcessibilidade
    {
        //controla quantos testes foram disparados
        private int quantidadeTestesDisparados;


        public async Task<List<ResultadoValidacao>> ValidarAcessibilidade(string Url)
        {

            using (IWebDriver driver = new ChromeDriver())
            {
                try
                {


                    driver.Navigate().GoToUrl(Url);
                    System.Threading.Thread.Sleep(5000);

                    var axe = new AxeBuilder(driver);

                    var validacaoAcessibilidade = axe.Analyze();

                    if (validacaoAcessibilidade.Violations.Length == 0)
                        return null;
                    


                    

                    return MappersResults.MapperToResultadoValidacao(validacaoAcessibilidade, Url);
                }
                finally
                {
                    driver.Quit();
                }

            }
        }


        public string ObterAplicabilidadeDeCodigo(string id)
        {
            var obtemBaseModeloIA = GerarModeloIA();

            var filtraErroPorId = obtemBaseModeloIA.FirstOrDefault(x => x.ID == id);

            if (filtraErroPorId == null)
            {
                return null;
            }

            return filtraErroPorId.ExemploDeCodigoHtml;
        }


        public string ObterDescricaoErro(string id)
        {
            var obtemBaseModeloIA = GerarModeloIA();

            var filtraErroPorId = obtemBaseModeloIA.FirstOrDefault(x => x.ID == id);

            if(filtraErroPorId==null)
            {
                return null;
            }

            return filtraErroPorId.DescricaoGeradaPorIA;
        }

        private List<BaseIA> GerarModeloIA()
        {
            string caminho = @"PocAutomacaoAcessibilidade.Domain\DDTOS\Data\BaseIA.json";
            string arquivoModeloIA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, caminho);
            string jsonContent = File.ReadAllText(arquivoModeloIA);

            List<BaseIA> obtemModeloIA = JsonConvert.DeserializeObject<List<BaseIA>>(jsonContent);

            return obtemModeloIA;

        }


    }
}
