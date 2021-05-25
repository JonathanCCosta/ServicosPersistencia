using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleImportacao.Service;
using ConsoleImportacao.Model;
using ConsoleImportacao.Proxy;
using System.Xml;
using System.Net;
using System.Globalization;
using System.Configuration;
using ConsoleImportacao.Utilities;
using ConsoleImportacao;


namespace ConsoleImport
{
    class Program
    {
        static void Main(string[] args)
        {

            string urlGovernanca = "http://shpt12:160/secretariagovernanca/";
            string urlSPE = "http://shpt12:200/dnnet/sp/gep/gestaospe/";
            string urlSPEDev = "http://dn-furnasnetd/dnnet/sp/gep/gestaospe/";
            string urlSENuvem = "http://www.sisspehml.cepel.br/dnnet/sp/gep/gestaospe/_vti_bin/ListData.svc/SPE";
            string urlSiss = "http://www.sisspehml.cepel.br/dnnet/sp/gep/gestaospe/SitePages/Default.aspx";
            GovernancaConsolidaSPE.Lists client = new GovernancaConsolidaSPE.Lists();
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            bool comErro = false;

            //Usuario usuario = new Usuario() { Dominio = ConfigurationManager.AppSettings["dominio"], Login = ConfigurationManager.AppSettings["usuario"], Senha = ConfigurationManager.AppSettings["senha"] };
            Usuario usuario = new Usuario() { Dominio = "sisspe", Login = "Furnas_fn", Senha = "Furn@321" };

            //client.Credentials = new NetworkCredential("tstfurnas_fn", "Modelo@098", "sisspe");
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            //  XmlNode listItemsNuvem = client.GetListItems("SPE", null, null, viewFields, null, null, null);


            /* List<Logo> logos = new Logo().GetLogo(client, usuario);
           
            bool retrono = new Logo().InsertAllLogo(logos, urlGovernanca );
         
           
           
             */
            List<Empreendimento> empreendimentos1 = new Empreendimento().GetEmpreendimentos(client, usuario);
            List<SPListItem> listGovernancaSPE = new Governanca().GetListGovernancaSPE();

            List<Governanca> lslGovernanca = new List<Governanca>();
            Governanca governanca;


            List<Complexo> complexos = new Complexo().GetALLComplexos(client, urlGovernanca, usuario);

            if (complexos.Count == 0)
            {
                bool primeiroInsertComplexos = new Complexo().InsertAllComplexoGOV(complexos, urlGovernanca);
            }

            List<SPE> listSPE = new SPE().GetListSPE(client, usuario);
            // List<SPE> listSPE = new SPE().GetListSPEByID(client, 92, usuario);

            foreach (ConsoleImportacao.SPE itemSPE in listSPE)
            {

                governanca = new Governanca();
                // C_x00d3_DIGO_x0020_DA_x0020_SPE backlog

                SPListItem itemGovernanca = listGovernancaSPE.Where(x => x.Title.ToString() == itemSPE.Title.ToString()).Cast<SPListItem>().FirstOrDefault();
                bool insert = (itemGovernanca == null);
                // trocar depois da carga

                ///  SPE  CNPJ DATA DE CONSTITUIÇÃO
                governanca.Title = itemSPE.Title;
                governanca.CNPJ = itemSPE.CNPJ;
                governanca.DataConstituicao = itemSPE.DataConstituicao;


                ///  SPE CONTATO   
                Contato contato = new Contato().GetContato(client, itemSPE, usuario);
                if (contato != null)
                    governanca.Site = contato.Title;


                /// COMPLEXO E LOGO SPE 
                Complexo complexo = new Complexo().GetComplexo(client, itemSPE, urlGovernanca, usuario);
                governanca.ComplexoDescricao = complexo.Descricao;
                governanca.Complexo = new ComplexoService().GetComplexo(complexo);
                governanca.Logo = new ComplexoService().GetLogo(complexo);
                governanca.IDComplexoGov = new Complexo().GetIDComplexoGOV(urlGovernanca, complexo);


                /// ESCRITORIO SPE 
                EscritorioSPE escritorioSPE = new EscritorioSPE().GetEscritorio(client, itemSPE, usuario);
                governanca.Endereco = new EscritorioSPEService().SetEscritorio(escritorioSPE);


                /// PARTICIPAÇÃO SPE 
                List<ParticipacaoSPE> participacaoSPE = new ParticipacaoSPE().GetParticipacoes(client, itemSPE, usuario);
                if (participacaoSPE.Count() > 0)
                    governanca.ParticipacoesSocios = new ConsoleImportacao.Service.ParticipacaoSPEService().SetParticipacao(participacaoSPE);

                //DESCRIÇÃO DO EMPREENDIMENTO 
                List<SPE> spesPorComplexo = new SPE().GetListSPEByComplexo(client, usuario, complexo);




                List<Empreendimento> empreendimentosPorComplexo = new Empreendimento().GetEmpreendimentos(empreendimentos1, spesPorComplexo);


                List<Empreendimento> empreendimentos = new List<Empreendimento>();
                Empreendimento emprendimentoTemp = null;
                string fase = string.Empty;
                KeyValuePair<int, DateTime?> maiorPesoDataEmpreendimento = new KeyValuePair<int, DateTime?>();
                if (empreendimentosPorComplexo != null)
                {
                    foreach (Empreendimento emprendimento in empreendimentosPorComplexo)
                    {
                        if (emprendimento.ID == 62)
                        {

                        }

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

                        //governanca.FaseEmpreendimento = emprendimento.FaseEmpreendimento;

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


                using (SPSite site = new SPSite(urlGovernanca))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        try
                        {
                            SPList list = web.Lists.TryGetList("SPE");
                            List<SPListItem> SPEs = new SPE().GetSPEGOV(urlGovernanca, itemSPE);


                            if (SPEs.Count == 0)
                            {
                                SPListItem NewItem = list.Items.Add();
                                {
                                    web.AllowUnsafeUpdates = true;


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


                                    if (governanca.Site != null)
                                    {
                                        SPFieldUrlValue hyper = new SPFieldUrlValue();
                                        hyper.Description = governanca.Title;
                                        hyper.Url = string.Format("{0}{1}", "http:///", governanca.Site).Replace(" ", "%20");
                                        NewItem["Site"] = hyper;
                                    }

                                    if (complexo.LogoID != 0)
                                    {
                                        SPFieldLookupValue logo = new SPFieldLookupValue(complexo.LogoID, governanca.Logo);
                                        NewItem["Logo"] = governanca.Logo;
                                    }
                                    NewItem["ImagemSPE"] = null;
                                    NewItem["Investimento"] = governanca.Investimento;
                                    NewItem["DataEntradaOperacaoPlena"] = governanca.DataEntradaOperacaoPlena;
                                    governanca.DocumentoBP = new DocumentosBalancoPatrimonia().GetDocumentosBalancoPatrimonial(client, itemSPE, usuario);
                                    if (governanca.DocumentoBP != null && governanca.DocumentoBP.LinkFilename != null && governanca.DocumentoBP.Response != null)
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
                                            NewItem.Attachments.Add(governanca.DocumentoBP.LinkFilename, governanca.DocumentoBP.Response);
                                    }
                                    NewItem.Update();
                                    //NewItem.Update();
                                }
                            }
                            if (SPEs.Count == 1)
                            {
                                web.AllowUnsafeUpdates = true;


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


                                if (governanca.Site != null)
                                {
                                    SPFieldUrlValue hyper = new SPFieldUrlValue();
                                    hyper.Description = governanca.Title;
                                    hyper.Url = string.Format("{0}{1}", "http:///", governanca.Site).Replace(" ", "%20");
                                    itemGovernanca["Site"] = hyper;
                                }

                                if (complexo.LogoID != 0)
                                {
                                    SPFieldLookupValue logo = new SPFieldLookupValue(complexo.LogoID, governanca.Logo);
                                    itemGovernanca["Logo"] = governanca.Logo;
                                }
                                itemGovernanca["ImagemSPE"] = null;
                                itemGovernanca["Investimento"] = governanca.Investimento;
                                itemGovernanca["DataEntradaOperacaoPlena"] = governanca.DataEntradaOperacaoPlena;
                                governanca.DocumentoBP = new DocumentosBalancoPatrimonia().GetDocumentosBalancoPatrimonial(client, itemSPE, usuario);
                                if (governanca.DocumentoBP != null && governanca.DocumentoBP.LinkFilename != null && governanca.DocumentoBP.Response != null)
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

                            web.AllowUnsafeUpdates = false;

                        }
                        catch (Exception e)
                        {
                            if (!comErro)
                                LogHelper.GravarLog(web, e);

                            comErro = true;
                        }
                    }
                }

                /*
                   
                                       itemGovernanca["Complexo"] = itemSPE["Complexo"];
                                       itemGovernanca["Title"] = itemSPE["Title"];
                                       itemGovernanca["CNPJ"] = itemSPE["CNPJ"];
                                       itemGovernanca["Endereco"] = governanca.Endereco;
                                       itemGovernanca["DataConstituicao"] = itemSPE["Data_x0020_de_x0020_Constitui_x0"];
                                       itemGovernanca["ParticipacoesSocios"] = governanca.ParticipacoesSocios;
                                       itemGovernanca["DescricaoEmpreendimento"] = governanca.DescricaoEmpreendimento;
                                       itemGovernanca["Diretoria"] = governanca.Diretoria;
                                       itemGovernanca["ConselhoAdministracao"] = governanca.ConselhoAdministracao;
                                       itemGovernanca["Estagio"] = governanca.Estagio;
                                       itemGovernanca["Site"] = null;
                                       itemGovernanca["Logo"] = governanca.Logo;
                                       itemGovernanca["ImagemSPE"] = null;

                                       itemGovernanca["Investimento"] = governanca.Investimento;*/
                // itemGovernanca["DataEntradaOperacaoPlena"] = governanca.DataEntradaOperacaoPlena;





                // programar os itens que tem em spe mas não tem em governaça
                //

            }



        }


    }
}

