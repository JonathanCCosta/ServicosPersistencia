using FURNAS.GestaoSPE.PersistenciaGovernanca.Model;
using FURNAS.GestaoSPE.PersistenciaGovernanca.Proxy;
using FURNAS.GestaoSPE.PersistenciaGovernanca.Service;
using FURNAS.GestaoSPE.GovernancaDados.Utilities;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Xml;
using FURNAS.GestaoSPE.GovernancaDados.Service;

namespace FURNAS.GestaoSPE.GovernancaDados.TimerJob
{
    public class JobMensal
    {
        public void Executar()
        {
            var urlGovernanca = Convert.ToString(Utilities.URLsAmbiente.HOM); //Convert.ToString("http://shpt12:160/secretariagovernanca");// apenas para teste
            string urlSISSPE = Utilities.URLsAmbiente.HOM_SISSPE;
            List<SPE> listSPE = null;
            Governanca governanca;
            List<Empreendimento> empreendimentos1 = null;
            List<SPListItem> listGovernancaSPE = null;
            List<Governanca> lslGovernanca;
            Usuario usuario = null;
            GovernancaConsolidaSPE.Lists client = null;

            SPSite site = new SPSite(urlGovernanca);
            SPWeb web = site.OpenWeb();

            Logo logoSISSPE = new Logo();
            List<Logo> logosSISSPE = null;
            try
            { 
                LogHelper.GravarLog(web, null, "Job Mensal Começou");
                    
                client = new GovernancaConsolidaSPE.Lists();
                XmlDocument xmlDoc = new System.Xml.XmlDocument();

                usuario = new Usuario() { Dominio = "sisspe", Login = "furnas_fn", Senha = "Furn@321" };
                client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
                XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
               
                /*----- PEGA COMPLEXO E SPE DA GOVERNANÇA -------*/
                List<Complexo> complexos = new Complexo().GetALLComplexosGov(client, urlGovernanca, usuario);
                listGovernancaSPE = new Governanca().GetListGovernancaSPE(urlGovernanca);

                /* ----- COMPLEXO E SPE DO SISTEMA SPE ----------*/
                  empreendimentos1 = new Empreendimento().GetEmpreendimentos(client, usuario);
                  List<Complexo> ComplexosSISSPE = new Complexo().GetALLComplexos(client, urlGovernanca, usuario);
                  listSPE = new SPE().GetListSPE(client, usuario);

                  /* ------- DELETA E INSERI LOGOS DA SPE -------*/

                  SPDocumentLibrary library = web.Lists["Logo SPE"] as SPDocumentLibrary;
                  logosSISSPE = logoSISSPE.GetLogo(client, urlSISSPE, usuario);
                  web.AllowUnsafeUpdates = true;

                  SPListItemCollection collectionLogo = library.GetItems();
                  for (int i = library.ItemCount - 1; i >= 0; i--)
                  {
                        collectionLogo[i].Delete();
                  }

                  foreach (Logo logoNew in logosSISSPE)
                  {
                      SPFile file = library.RootFolder.Files.Add(logoNew.LinkFilename, logoNew.Response, true);
                      library.Update();

                      logoNew.ID = file.Item.ID;
                  }
                  web.AllowUnsafeUpdates = false;

                 lslGovernanca = new List<Governanca>();

                /* --------- INSERI TODOS OS COMPLEXOS CASO NÃO EXISTIR */
                if (complexos.Count == 0)
                {
                    bool primeiroInsertComplexos = new Complexo().InsertAllComplexoGOV(ComplexosSISSPE, urlGovernanca);
                }
            }
            catch (Exception se)
            {
               LogHelper.GravarLog(web, se, "catch 1 - Microsoft.SharePoint.SoapServer.SoapServerException se");
            }

            foreach (SPE itemSPE in listSPE)
            {
                Complexo complexo = null;
                SPListItem itemGovernanca = null;
                governanca = new Governanca();
                try
                {
                    itemGovernanca = listGovernancaSPE.Where(x => x.Title.ToString() == itemSPE.Title.ToString()).Cast<SPListItem>().FirstOrDefault();
                    bool insert = (itemGovernanca == null);
                    // trocar depois da carga
                    if (itemGovernanca != null)
                    {
                        ///  SPE  CNPJ DATA DE CONSTITUIÇÃO
                        governanca.Title = itemSPE.Title;
                        governanca.CNPJ = itemSPE.CNPJ;
                        governanca.DataConstituicao = itemSPE.DataConstituicao;
                        governanca.ID_SPE = itemSPE.ID;
                        ///  SPE CONTATO   
                        Contato contato = new Contato().GetContato(client, itemSPE, usuario);
                        if (contato != null)
                            governanca.Site = contato.Title;


                        /// COMPLEXO E LOGO SPE 
                        complexo = new Complexo().GetComplexo(client, itemSPE, urlGovernanca, usuario);

                        if (complexo != null)
                        {
                            governanca.ComplexoDescricao = complexo.Descricao;
                            governanca.Complexo = new ComplexoService().GetComplexo(complexo);
                            governanca.IDComplexoGov = new Complexo().GetIDComplexoGOV(urlGovernanca, complexo);
                            governanca.Logo = new ComplexoService().GetLogo(complexo);

                            /// ESCRITORIO SPE 
                            EscritorioSPE escritorioSPE = new EscritorioSPE().GetEscritorio(client, itemSPE, usuario);
                            governanca.Endereco = new EscritorioSPEService().SetEscritorio(escritorioSPE);


                            /// PARTICIPAÇÃO SPE 
                            List<ParticipacaoSPE> participacaoSPE = new ParticipacaoSPE().GetParticipacoes(client, itemSPE, usuario);
                            if (participacaoSPE.Count() > 0)
                                governanca.ParticipacoesSocios = new ParticipacaoSPEService().SetParticipacao(participacaoSPE);

                            //DESCRIÇÃO DO EMPREENDIMENTO 
                            List<SPE> spesPorComplexo = new SPE().GetListSPEByComplexo(client, usuario, complexo);



                            // EMPREENDIMENTO
                            List<Empreendimento> empreendimentosPorComplexo = new Empreendimento().GetEmpreendimentos(empreendimentos1, spesPorComplexo);


                            List<Empreendimento> empreendimentos = new List<Empreendimento>();
                            Empreendimento emprendimentoTemp = null;
                            string fase = string.Empty;
                            KeyValuePair<int, DateTime?> maiorPesoDataEmpreendimento = new KeyValuePair<int, DateTime?>();
                            if (empreendimentosPorComplexo != null)
                            {
                                foreach (Empreendimento emprendimento in empreendimentosPorComplexo)
                                {

                                    Obra obra = new Obra(client, emprendimento, usuario);
                                    emprendimento.Obra = new List<Obra>();

                                    KeyValuePair<int, DateTime?> maiotPesoDataObra = new KeyValuePair<int, DateTime?>();
                                    foreach (Obra itemObra in obra.itensObras)
                                    {
                                        itemObra.PesoData = new KeyValuePair<int, DateTime?>();
                                        var atividade = new Atividades().GetDataMaiorAtividade(client, itemObra, usuario);
                                        maiotPesoDataObra = itemObra.PesoData = new AtividadesService().CompararData(maiotPesoDataObra, atividade);
                                        emprendimento.Obra.Add(itemObra);
                                    }

                                    emprendimento.PesoData = new KeyValuePair<int, DateTime?>();
                                    emprendimento.PesoData = new AtividadesService().CompararData(maiorPesoDataEmpreendimento, maiotPesoDataObra);
                                    maiorPesoDataEmpreendimento = emprendimento.PesoData;

                                    governanca.DataEntradaOperacaoPlena = maiorPesoDataEmpreendimento.Value;
                                    emprendimentoTemp = new EmpreendimentoService().VerificarFaseAnterior(emprendimentoTemp, emprendimento, out fase);
                                    governanca.FaseEmpreendimento = fase == string.Empty ? emprendimentoTemp.FaseEmpreendimento : fase;
                                }

                            }

                            //DIRETORIA
                            List<Diretoria> diretorias = new Diretoria().GetDiretoria(client, itemSPE, usuario);
                            if (diretorias.Count() > 0)
                                governanca.Diretoria = new DiretoriaService().SetDiretoria(diretorias);


                            //CONSELHO DE ADMINISTRAÇÃO
                            List<ConselhoAdministracao> conselhoAdministracaos = new ConselhoAdministracao().GetConselhoAdministracao(client, itemSPE, usuario);
                            if (conselhoAdministracaos.Count() > 0)
                                governanca.ConselhoAdministracao = new ConselhoAdministracaoService().SetConselhoAdministracao(conselhoAdministracaos);

                            /// FLUXO DE CAIXA
                            List<FluxodeCaixa> fluxodeCaixa = new FluxodeCaixa().GetFluxodeCaixa(client, itemSPE, usuario);

                            if (fluxodeCaixa.Count() > 0)
                            {
                                /// ITEM FC
                                List<ItemFC> itemFC = new ItemFC().GetItemFC(client, fluxodeCaixa, usuario);
                                if (itemFC.Count() > 0)
                                    governanca.Investimento = new ItemFCService().SetFluxodeCaixa(itemFC);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.GravarLog(web, e, "catch 2 - Erro");
                }

                List<SPListItem> SPEs = null;
                try
                {
                    SPList list = web.Lists.TryGetList("SPE");
                    SPEs = new SPE().GetSPEGOV(urlGovernanca, itemSPE);
                    //web.AllowUnsafeUpdates = true;
                    if (SPEs.Count == 0)
                    {
                        SPListItem NewItem = list.Items.Add();
                        {
                            if (governanca.IDComplexoGov != 0)
                                NewItem["Complexo"] = new SPFieldLookupValue(governanca.IDComplexoGov, complexo.Title);


                            NewItem["Title"] = governanca.Title;
                            NewItem["CNPJ"] = governanca.CNPJ;
                            NewItem["Endereco"] = governanca.Endereco;
                            NewItem["DataConstituicao"] = governanca.DataConstituicao;
                            NewItem["ParticipacoesSocios"] = governanca.ParticipacoesSocios;
                            NewItem["DescricaoEmpreendimento"] = governanca.ComplexoDescricao;
                            NewItem["Diretoria"] = governanca.Diretoria;
                            NewItem["ConselhoAdministracao"] = governanca.ConselhoAdministracao;
                            NewItem["Estagio"] = governanca.FaseEmpreendimento;
                            NewItem["ID_SPE"] = governanca.ID_SPE;

                            if (governanca.Site != null)
                            {
                                SPFieldUrlValue hyper = new SPFieldUrlValue();
                                hyper.Description = governanca.Title;
                                hyper.Url = string.Format("{0}{1}", "http:///", governanca.Site).Replace(" ", "%20");
                                NewItem["Site"] = hyper;
                            }

                            if (complexo.LogoID != 0)
                            {
                                //SPFieldLookupValue logo = new SPFieldLookupValue(complexo.LogoID, governanca.Logo);
                                string nomeLogo = governanca.Logo.ToString().Split('/').LastOrDefault();
                                foreach (Logo l in logosSISSPE)
                                {
                                    if (l.LinkFilename == nomeLogo)
                                    {
                                        NewItem["Logo"] = l.ID + governanca.Logo.Remove(0, 2);//library.RootFolder.Url + "/"
                                        break;
                                    }
                                }
                                //NewItem["Logo"] = governanca.Logo;
                            }
                            //NewItem["ImagemSPE"] = null;
                            NewItem["Investimento"] = governanca.Investimento;
                            NewItem["DataEntradaOperacaoPlena"] = governanca.DataEntradaOperacaoPlena;
                            governanca.DocumentoBP = new DocumentosBalancoPatrimonia().GetDocumentosBalancoPatrimonial(client, itemSPE, usuario, urlSISSPE, web);
                              if (governanca.DocumentoBP != null && governanca.DocumentoBP.LinkFilename != null && governanca.DocumentoBP.Response != null)
                              {
                                  bool temAnexo = false;
                                  foreach (var file in itemGovernanca.Attachments)
                                  {
                                      if (file.ToString().ToLower() == governanca.DocumentoBP.LinkFilename.ToString().ToLower())
                                          temAnexo = true;
                                  }
                                  if (!temAnexo)
                                      NewItem.Attachments.Add(governanca.DocumentoBP.LinkFilename, governanca.DocumentoBP.Response);
                              }
                            NewItem.Update();
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.GravarLog(web, e, "Processo de Criação da SPE com Informações");
                }
                finally
                {
                   // web.AllowUnsafeUpdates = false;
                }

                try
                {
                   // web.AllowUnsafeUpdates = true;
                    if (SPEs.Count == 1)
                    {
                        if (governanca.IDComplexoGov != 0)
                            itemGovernanca["Complexo"] = new SPFieldLookupValue(governanca.IDComplexoGov, complexo.Title);
                        itemGovernanca["Title"] = governanca.Title;
                        itemGovernanca["CNPJ"] = governanca.CNPJ;
                        itemGovernanca["Endereco"] = governanca.Endereco;
                        itemGovernanca["DataConstituicao"] = governanca.DataConstituicao;
                        itemGovernanca["ParticipacoesSocios"] = governanca.ParticipacoesSocios;
                        itemGovernanca["DescricaoEmpreendimento"] = governanca.ComplexoDescricao;
                        itemGovernanca["Diretoria"] = governanca.Diretoria;
                        itemGovernanca["ConselhoAdministracao"] = governanca.ConselhoAdministracao;
                        itemGovernanca["Estagio"] = governanca.FaseEmpreendimento;
                        itemGovernanca["ID_SPE"] = governanca.ID_SPE;
                        if (governanca.Site != null)
                        {
                            SPFieldUrlValue hyper = new SPFieldUrlValue();
                            hyper.Description = governanca.Title;
                            hyper.Url = string.Format("{0}{1}", "http:///", governanca.Site).Replace(" ", "%20");
                            itemGovernanca["Site"] = hyper;
                        }

                        if (complexo != null)
                        {
                            if (complexo.LogoID != 0)
                            {
                                //SPFieldLookupValue logo = new SPFieldLookupValue(complexo.LogoID, governanca.Logo);
                                string nomeLogo = governanca.Logo.ToString().Split('/').LastOrDefault();
                                foreach (Logo l in logosSISSPE)
                                {
                                    if (l.LinkFilename == nomeLogo)
                                    {
                                        itemGovernanca["Logo"] = l.ID + governanca.Logo.Remove(0, 2);
                                        break;
                                    }
                                }
                            }
                        }
                        //itemGovernanca["ImagemSPE"] = null;
                        itemGovernanca["Investimento"] = governanca.Investimento;
                        itemGovernanca["DataEntradaOperacaoPlena"] = governanca.DataEntradaOperacaoPlena;
                          governanca.DocumentoBP = new DocumentosBalancoPatrimonia().GetDocumentosBalancoPatrimonial(client, itemSPE, usuario, urlSISSPE, web);
                          if (governanca.DocumentoBP != null)
                          {
                              bool temAnexo = false;
                              foreach (var file in itemGovernanca.Attachments)
                              {
                                  if (file.ToString().ToLower() == governanca.DocumentoBP.LinkFilename.ToString().ToLower())
                                  {
                                      temAnexo = true;
                                  }
                              }
                              if (!temAnexo)
                                  itemGovernanca.Attachments.Add(governanca.DocumentoBP.LinkFilename, governanca.DocumentoBP.Response);
                          }

                        itemGovernanca.Update();
                    }


                }
                catch (Exception e)
                {
                    LogHelper.GravarLog(web, e, "Processo de Update da SPE com Informações");
                }
                finally
                {
                    //web.AllowUnsafeUpdates = false;

                }
            }

            LogHelper.GravarLog(web, null, "Job Mensal Terminou");
            web.Close();
            site.Dispose();
        }
    }
}
