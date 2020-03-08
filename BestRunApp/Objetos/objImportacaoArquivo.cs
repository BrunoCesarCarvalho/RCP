using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;

namespace BestRunApp.Objetos
{
    public class propLayoutArquivo
    {
        public int iPosicao { get; set; }
        public DateTime? dtHora { get; set; }
        public string sCodPiloto { get; set; }
        public string sPiloto { get; set; }
        public int? iNumeroVolta { get; set; }
        public TimeSpan dtTempoVolta { get; set; }
        public decimal? dVelocidadeMedia { get; set; }
    }
    public class propResultLayoutArquivo
    {
        public int iPosicao { get; set; }
        public string sCodPiloto { get; set; }
        public string sPiloto { get; set; }
        public int? iNumeroVolta { get; set; }
        public TimeSpan tsTotalTempo { get; set; }
        public TimeSpan tsNumeroMelhorVolta { get; set; }
        public decimal? dAvgVelocidadeMedia { get; set; }
    }

    public class metodosImportacao
    {
        public List<propResultLayoutArquivo> ListDadosImportados(StreamReader srFile)
        {
            propResultLayoutArquivo propResult;
            List<propResultLayoutArquivo> listResultItens = new List<propResultLayoutArquivo>();

            propLayoutArquivo propItens;
            List<propLayoutArquivo> listItens = new List<propLayoutArquivo>();

            int iNumLinhaErro = 0;
            int iNumLinha = 0;
            string sLinha;
            string[] lsCampos;

            try
            {
                while ((sLinha = srFile.ReadLine()) != null)
                {
                    if (iNumLinha != 0 && sLinha != null)
                    {
                        try
                        {
                            lsCampos = new string[7];

                            lsCampos = sLinha.Split(' ');

                            propItens = new propLayoutArquivo();
                            propItens.dtHora = Convert.ToDateTime(lsCampos[0].PadLeft(12, '0'), new CultureInfo("pt-BR"));
                            propItens.sCodPiloto = lsCampos[1];
                            propItens.sPiloto = lsCampos[3];
                            propItens.iNumeroVolta = Convert.ToInt32(lsCampos[4]);
                            TimeSpan sTempo = (
                                lsCampos[5].Length == 8 ?
                                TimeSpan.Parse($"00:{lsCampos[5].PadLeft(9, '0')}", new CultureInfo("pt-BR")) :
                                TimeSpan.Parse(lsCampos[5].PadLeft(12, '0'), new CultureInfo("pt-BR"))
                                );
                            propItens.dtTempoVolta = sTempo;
                            propItens.dVelocidadeMedia = Convert.ToDecimal(lsCampos[6]);

                            listItens.Add(propItens);
                        }
                        catch (Exception e)
                        {
                            Log($"Erro na leitura da linha: {iNumLinha} - Erro: {e.Message}");
                            iNumLinhaErro++;
                            iNumLinha--;
                        }
                    }
                    iNumLinha++;
                }

                srFile.Close();

                Log($"Total de linhas processadas: {iNumLinha}");
                Log($"Total de linhas com erro: {iNumLinhaErro}");

                foreach (var item in listItens.Select(x => x.sCodPiloto).Distinct())
                {
                    propResult = new propResultLayoutArquivo();
                    propResult.iPosicao = 0;
                    propResult.sCodPiloto = listItens.First(x => x.sCodPiloto == item).sCodPiloto;
                    propResult.sPiloto = listItens.First(x => x.sCodPiloto == item).sPiloto;
                    propResult.iNumeroVolta = listItens.Where(x => x.sCodPiloto == item).Max(w => w.iNumeroVolta);
                    propResult.tsTotalTempo = listItens.Where(x => x.sCodPiloto == item).Sum(x => x.dtTempoVolta);
                    propResult.tsNumeroMelhorVolta = listItens.Where(x => x.sCodPiloto == item).Min(x => x.dtTempoVolta);
                    propResult.dAvgVelocidadeMedia = listItens.Where(x => x.sCodPiloto == item).Average(x => x.dVelocidadeMedia);
                    listResultItens.Add(propResult);
                }

                listResultItens = listResultItens.OrderBy(x => x.tsTotalTempo).ToList().Select((x, index) => new propResultLayoutArquivo
                {
                    iPosicao = index + 1,
                    sCodPiloto = x.sCodPiloto,
                    sPiloto = x.sPiloto,
                    iNumeroVolta = x.iNumeroVolta,
                    tsTotalTempo = x.tsTotalTempo,
                    tsNumeroMelhorVolta = x.tsNumeroMelhorVolta,
                    dAvgVelocidadeMedia = x.dAvgVelocidadeMedia

                }).ToList();

         
            }
            catch (Exception e)
            {
                Log($"Erro geral no processamento do arquivo. Consulte o suporte: Metodo de leitura do arquivo - erro: {e.Message}");
            }

            return listResultItens;
        }
        public void Log(string sMsg)
        {
            try
            {
                string Path = AppDomain.CurrentDomain.BaseDirectory;
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] -> " + sMsg);
                string sDirectory = Path + "\\LOG";
                string sFile_Name = DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(sDirectory))
                {
                    Directory.CreateDirectory(sDirectory);
                }

                sFile_Name = (sDirectory + ("\\" + sFile_Name + ".log"));
                FileInfo oFileInfo = new FileInfo(sFile_Name);
                StreamWriter oEscreve;
                if (File.Exists(sFile_Name))
                {
                    oEscreve = new StreamWriter(sFile_Name, true);
                    oEscreve.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] -> " + sMsg);
                    oEscreve.Flush();
                    oEscreve.Close();
                }
                else
                {
                    oEscreve = new StreamWriter(sFile_Name);
                    oEscreve.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] -> " + sMsg);
                    oEscreve.Flush();
                    oEscreve.Close();
                    oEscreve.Dispose();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void LoadGrid(List<propResultLayoutArquivo> listItens, DataGrid dtg)
        {

            if (listItens == null)
            {

                DataGridTextColumn DtCT;

                DtCT = new DataGridTextColumn();
                DtCT.Binding = new Binding("iPosicao");
                dtg.Columns.Add(DtCT);        
                DtCT = new DataGridTextColumn();
                DtCT.Binding = new Binding("sCodPiloto");
                dtg.Columns.Add(DtCT);
                DtCT = new DataGridTextColumn();
                DtCT.Binding = new Binding("sPiloto");
                dtg.Columns.Add(DtCT);
                DtCT = new DataGridTextColumn();
                DtCT.Binding = new Binding("iNumeroVolta");
                dtg.Columns.Add(DtCT);
                DtCT = new DataGridTextColumn();
                DtCT.Binding = new Binding("tsTotalTempo");
                dtg.Columns.Add(DtCT);
                DtCT = new DataGridTextColumn();
                DtCT.Binding = new Binding("tsNumeroMelhorVolta");
                dtg.Columns.Add(DtCT);
                DtCT = new DataGridTextColumn();
                DtCT.Binding = new Binding("dAvgVelocidadeMedia");
                dtg.Columns.Add(DtCT);
            }
            else
            {
                dtg.Columns.Clear();

                dtg.ItemsSource = listItens;
            }

            dtg.Columns[0].Header = "Posição chegada";
            dtg.Columns[0].Width = 100;
            dtg.Columns[1].Header = "Código Piloto";
            dtg.Columns[1].Width = 100;
            dtg.Columns[2].Header = "Nome Piloto";
            dtg.Columns[2].Width = 275;
            dtg.Columns[3].Header = "Qtde Voltas Completadas";
            dtg.Columns[3].Width = 150;
            dtg.Columns[4].Header = "Tempo Total de Prova";
            dtg.Columns[4].Width = 150;
            dtg.Columns[5].Header = "Melhor volta (Tempo)";
            dtg.Columns[5].Width = 150;
            dtg.Columns[6].Header = "Velocidade média (km/h)";
            dtg.Columns[6].Width = 150;


        }
    }
    public static class LinqExtensions
    {
        public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> func)
        {
            return new TimeSpan(source.Sum(item => func(item).Ticks));
        }
    }
}
