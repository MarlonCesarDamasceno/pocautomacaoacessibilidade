using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Mappers;
using Selenium.Axe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Infra
{
    public class MotorAcessibilidade: IMotorAcessibilidade
    {

        public async Task<List<ResultadoValidacao>>  ValidarAcessibilidade(string Url)
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
                    var saida = JsonSerializer.Serialize(validacaoAcessibilidade);
                    Console.WriteLine("Saida bruta: " + saida);
                    

                    return MappersResults.MapperToResultadoValidacao(validacaoAcessibilidade);
                }
                finally
                {
                    driver.Quit();
                }

            }
        }

    }
}
