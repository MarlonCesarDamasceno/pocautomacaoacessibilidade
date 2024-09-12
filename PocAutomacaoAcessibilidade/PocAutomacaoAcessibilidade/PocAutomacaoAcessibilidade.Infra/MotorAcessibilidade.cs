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
        //controla quantos testes foram disparados
        private int quantidadeTestesDisparados;


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

                    quantidadeTestesDisparados++;

                    return MappersResults.MapperToResultadoValidacao(validacaoAcessibilidade, Url, quantidadeTestesDisparados);
                    //zera a quantidade de testes.
                    quantidadeTestesDisparados = 0;
                }
                finally
                {
                    driver.Quit();
                }

            }
        }

    }
}
