using FURNAS.GestaoSPE.PersistenciaGovernanca.Model;
using FURNAS.GestaoSPE.PersistenciaGovernanca.Proxy;
using FURNAS.GestaoSPE.PersistenciaGovernanca.Service;
using FURNAS.GestaoSPE.PersistenciaGovernanca.Utilities;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.TimerJob
{
    public class JobDiario
    {
        public void Executar()
        {
            string urlGovernanca = Utilities.URLsAmbiente.HOM; //"http://dp-furnasnetd/secretariagovernanca/"; //"http://shpt12:160/secretariagovernanca";
            SPSite site = new SPSite(urlGovernanca);
            SPWeb web = site.OpenWeb();
            LogHelper.GravarLog(web, null, "JobDiario - Começou");

            GovernancaConsolidaSPE.Lists client = new GovernancaConsolidaSPE.Lists();
            XmlDocument xmlDoc = new System.Xml.XmlDocument();

            Usuario usuario = new Usuario() { Dominio = "sisspe", Login = "furnas_fn", Senha = "Furn@321" };
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");

            // --- LISTAS DE SISSPE
            List<Empreendimento> empreendimentos1 = new Empreendimento().GetEmpreendimentos(client, usuario);
            List<Complexo> complexos = new Complexo().GetALLComplexos(client, urlGovernanca, usuario);

            // --- LISTAS DE GOVERNANÇA
            List<SPListItem> listGovernancaSPE = new Governanca().GetListGovernancaSPE(urlGovernanca);
            List<Complexo> listGovernancaComplexo = new Complexo().GetALLComplexosGov(client,urlGovernanca,usuario);

            List<Governanca> lslGovernanca = new List<Governanca>();
            Governanca governanca;
            
            // SE NÃO HOUVER COMPLEXOS CARREGA TODOS
            /*if (complexos.Count == 0)
            {
                bool primeiroInsertComplexos = new Complexo().InsertAllComplexoGOV(complexos, urlGovernanca);
            }*/

            //SE O NUMEROS DE COMPLEXOS FOR DIFERENTE ADICIONA OS NÃO EXISTENTES
            if (complexos.Count > listGovernancaComplexo.Count)
            {
                foreach(Complexo complexoItem in complexos)
                {
                    try
                    {
                        bool CmpGov = listGovernancaComplexo.Exists(x => x.Title.ToString() == complexoItem.Title.ToString());//.Cast<SPListItem>().FirstOrDefault();
                        if (CmpGov == false)
                        {
                            web.AllowUnsafeUpdates = true;
                            SPList list = web.Lists.TryGetList("Complexo SPE");
                            SPListItem NewItem = list.Items.Add();
                            NewItem["Complexo"] = complexoItem.Title;
                            NewItem["DescricaoComplexo"] = (complexoItem.Descricao == null ? "" : complexoItem.Descricao);
                            NewItem["ID_Complexo"] = complexoItem.ID;

                            NewItem.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                    catch(Exception ex)
                    {
                        LogHelper.GravarLog(web, ex, "Inserir um Complexo da SPE");
                    }
                }
            }
            else if (complexos.Count < listGovernancaComplexo.Count)
            {
                //SPListItem itemComplexo = null;
                foreach (Complexo complexoItem in listGovernancaComplexo)
                {
                    try
                    {
                        bool itemComplexo = complexos.Exists(x => x.Title.ToString() == complexoItem.Title.ToString());//.Cast<SPListItem>().FirstOrDefault();
                        if (itemComplexo == false)
                        {
                            SPList list = web.Lists.TryGetList("Complexo SPE");
                            web.AllowUnsafeUpdates = true;
                            list.GetItemById(complexoItem.ID).Delete();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.GravarLog(web, ex, "Deletar um Complexo da Governança");
                    }
                }
            }

            List<SPE> listSPE = new SPE().GetListSPE(client, usuario);
            // List<SPE> listSPE = new SPE().GetListSPEByID(client, 92, usuario);

            foreach (SPListItem itemGovSPE in listGovernancaSPE.OrderByDescending(x => x.Title))
            {
                bool itemGovernancaSPE = listSPE.Exists(x => x.Title.ToString() == itemGovSPE.Title.ToString()); //Where(x => x.Title.ToString() == itemGovSPE.Title.ToString()).Cast<SPListItem>().FirstOrDefault();
                if (itemGovernancaSPE == false)
                {
                    web.AllowUnsafeUpdates = true;
                    itemGovSPE.Delete();
                    web.AllowUnsafeUpdates = false;
                }
            }

            foreach (SPE itemSPE in listSPE)
            {
                governanca = new Governanca();
                // C_x00d3_DIGO_x0020_DA_x0020_SPE backlog

                SPListItem itemGovernanca = listGovernancaSPE.Where(x => x.Title.ToString() == itemSPE.Title.ToString()).Cast<SPListItem>().FirstOrDefault();
                //bool insert = (itemGovernanca == null);
                // trocar depois da carga

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
                Complexo complexo = new Complexo().GetComplexo(client, itemSPE, urlGovernanca, usuario);
                if (complexo != null)
                {
                    governanca.ComplexoDescricao = complexo.Descricao;
                    governanca.Complexo = new ComplexoService().GetComplexo(complexo);
                    //governanca.Logo = new ComplexoService().GetLogo(complexo);
                    governanca.IDComplexoGov = new Complexo().GetIDComplexoGOV(urlGovernanca, complexo);

                    /// ESCRITORIO SPE 
                    EscritorioSPE escritorioSPE = new EscritorioSPE().GetEscritorio(client, itemSPE, usuario);
                    governanca.Endereco = new EscritorioSPEService().SetEscritorio(escritorioSPE);


                    /// PARTICIPAÇÃO SPE 
                    List<ParticipacaoSPE> participacaoSPE = new ParticipacaoSPE().GetParticipacoes(client, itemSPE, usuario);
                    if (participacaoSPE.Count() > 0)
                        governanca.ParticipacoesSocios = new ParticipacaoSPEService().SetParticipacao(participacaoSPE);

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
                                NewItem["ImagemSPE"] = null;
                                NewItem["Investimento"] = governanca.Investimento;
                                NewItem["DataEntradaOperacaoPlena"] = governanca.DataEntradaOperacaoPlena;
                                NewItem["ID_SPE"] = governanca.ID_SPE;
                                NewItem.Update();
                                web.AllowUnsafeUpdates = false;
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
                            //itemGovernanca["ImagemSPE"] = null;
                            itemGovernanca["Investimento"] = governanca.Investimento;
                            itemGovernanca["DataEntradaOperacaoPlena"] = governanca.DataEntradaOperacaoPlena;
                            itemGovernanca["ID_SPE"] = governanca.ID_SPE;

                            itemGovernanca.Update();

                            web.AllowUnsafeUpdates = false;
                        }

                    }
                    catch (Exception e)
                    {
                        LogHelper.GravarLog(web, e, "Erro ao Atualizar ou Inserir SPE");
                    }

                }
            }
            LogHelper.GravarLog(web, null, "JobDiario - Terminou");
            web.Close();
            site.Dispose();
        }
    }
}
