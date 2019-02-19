using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Perfan
{
    public partial class Inicio : Form
    {
        FolderBrowserDialog diretorio = new FolderBrowserDialog();
        Process processo;

        public Inicio()
        {
            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            btnParar.Enabled = false;
            btnReiniciar.Enabled = false;
            txbDiretorio.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void BtnDiretorio_Click(object sender, EventArgs e)
        {
            if (diretorio.ShowDialog() == DialogResult.OK)
            {
                txbDiretorio.Text = diretorio.SelectedPath;
                try
                {
                    if (Arquivo.GravarLog(" ", Path.Combine(txbDiretorio.Text, "teste.dat")) == -1)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    File.Delete(Path.Combine(txbDiretorio.Text, "teste.dat"));
                }
                catch
                {
                    txbDiretorio.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    MessageBox.Show("Pasta sem permissão para gravar arquivos, favor dar permissão ou escolher outro diretório.", "Erro de Permissão para Gravar em Diretório:");
                    txbDiretorio.Focus();
                }
            }

        }

        private void BtnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txbProcesso.Text.Length < 1)
                {
                    MessageBox.Show("Favor informar o número PID do processo.", "Campo vazio");
                    txbProcesso.Focus();
                }
                else
                {
                    if (Convert.ToInt32(txbProcesso.Text) > 0)
                    {
                        try
                        {
                            processo = Process.GetProcessById(Convert.ToInt32(txbProcesso.Text));

                            if (MessageBox.Show("PID: " + processo.Id + "\nProcesso: " + processo.ProcessName + "\n\nO processo listado está correto?", "Confirmação:", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                btnIniciar.Enabled = false;
                                btnParar.Enabled = true;
                                btnReiniciar.Enabled = false;
                                lblProcesso.Visible = false;
                                lblDiretorio.Visible = false;
                                txbProcesso.Visible = false;
                                txbProcesso.Enabled = false;
                                txbDiretorio.Visible = false;
                                txbDiretorio.Enabled = false;
                                btnDiretorio.Visible = false;
                                btnDiretorio.Enabled = false;
                                txbLog.Visible = true;
                                pgrLog.Visible = true;
                                pgrLog.Style = ProgressBarStyle.Marquee;
                                pgrLog.MarqueeAnimationSpeed = 30;

                                txbLog.Text += "Processo: " + processo.Id + "(" + processo.ProcessName + ")\n\n";
                                txbLog.Text += "HH:mm:ss\tMBytes (Conjunto de Trabalho)\n";
                                timerLog.Enabled = true;
                                timerLog.Start();

                                /*
                                txbLog.Text = "                  Processo: " + processo.Id + " (" + processo.ProcessName + ")\n" +
                                    "\n       PrivateMemorySize64: " + processo.PrivateMemorySize64 / 1024 + "KB\t\t" + (int)(processo.PrivateMemorySize64 / 1024) / 1024 + "MB" +
                                    "\n              WorkingSet64: " + processo.WorkingSet64 / 1024 + "KB\t" + (int)(processo.WorkingSet64 / 1024) / 1024 + "MB" +
                                    "\n     PeakPagedMemorySize64: " + processo.PeakPagedMemorySize64 / 1024 + "KB\t\t" + (int)(processo.PeakPagedMemorySize64 / 1024) / 1024 + "MB" +
                                    "\nNonpagedSystemMemorySize64: " + processo.NonpagedSystemMemorySize64 / 1024 + "KB\t\t" + (int)(processo.NonpagedSystemMemorySize64 / 1024) / 1024 + "MB" +
                                    "\n         PagedMemorySize64: " + processo.PagedMemorySize64 / 1024 + "KB\t\t" + (int)(processo.PagedMemorySize64 / 1024) / 1024 + "MB" +
                                    "\n   PagedSystemMemorySize64: " + processo.PagedSystemMemorySize64 / 1024 + "KB\t\t" + (int)(processo.PagedSystemMemorySize64 / 1024) / 1024 + "MB" +
                                    "\n   PeakVirtualMemorySize64: " + processo.PeakVirtualMemorySize64 / 1024 + "KB\t" + (int)(processo.PeakVirtualMemorySize64 / 1024) / 1024 + "MB" +
                                    "\n       VirtualMemorySize64: " + processo.VirtualMemorySize64 / 1024 + "KB\t" + (int)(processo.VirtualMemorySize64 / 1024) / 1024 + "MB" +
                                    "\n          PeakWorkingSet64: " + processo.PeakWorkingSet64 / 1024 + "KB\t" + (int)(processo.PeakWorkingSet64 / 1024) / 1024 + "MB";
                                */
                            }
                            else
                            {
                                txbProcesso.Focus();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("O PID informado não corresponde a nenhum processo em execução!\n\nPor favor, tente novamente...\n\nDetalhes:\n" + ex, "PID Inexistente");
                            txbProcesso.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("O PID informado não está em execução ou é inválido!\n\nPor favor, corrija-o e tente novamente...", "Erro com número PID");
                        txbProcesso.Text = "";
                        txbProcesso.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("O valor informado no campo 'Processo (PID)' é inválido! Ele deve ser um número inteiro maior que 0.\n\nPor favor, corrija-o e tente novamente...\n\nDetalhes:\n" + ex, "Erro com número PID");
                txbProcesso.Text = "";
                txbProcesso.Focus();
            }


        }

        private void TimerLog_Tick(object sender, EventArgs e)
        {
            try
            {
                processo.Refresh();
                txbLog.Text += DateTime.Now.ToString("HH:mm:ss") + "\t" + Math.Round((processo.WorkingSet64 / 1024.0) / 1024.0) + "\n";
                txbLog.Focus();
                txbLog.SelectionStart = txbLog.Text.Length;
                txbLog.ScrollToCaret();
            }
            catch
            {
                timerLog.Stop();
                timerLog.Enabled = false;
                btnParar.Enabled = false;
                btnReiniciar.Enabled = true;
                pgrLog.Visible = false;

                //SalvarLog();
                
                Arquivo.GravarLog(txbLog.Text, Path.Combine(txbDiretorio.Text, "PERFAN_LOG_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".dat"));
                txbLog.Text += "Processamento interrompido pela finalização do processo e log salvo com sucesso!\n";
                txbLog.Focus();
                txbLog.SelectionStart = txbLog.Text.Length;
                txbLog.ScrollToCaret();
            }
        }

        private void BtnParar_Click(object sender, EventArgs e)
        {
            try
            {
                timerLog.Stop();
                timerLog.Enabled = false;
                btnParar.Enabled = false;
                btnReiniciar.Enabled = true;
                pgrLog.Visible = false;

                //SalvarLog();
                if (Arquivo.GravarLog(txbLog.Text, Path.Combine(txbDiretorio.Text, "PERFAN_LOG_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".dat")) == -1)
                {
                    throw new UnauthorizedAccessException();
                }
                txbLog.Text += "Processamento finalizado e log salvo com sucesso!\n";
                txbLog.Focus();
                txbLog.SelectionStart = txbLog.Text.Length;
                txbLog.ScrollToCaret();
            }
            catch
            {
                MessageBox.Show("Ocorreu um erro ao salvar arquivo de log no diretório abaixo, favor verificar as permissões da pasta e tentar novamente.\nDiretório: " + txbDiretorio.Text, "Erro Fatal ao Salvar o Log");
            }
        }

        private void BtnReiniciar_Click(object sender, EventArgs e)
        {
            txbLog.Text = "";
            txbProcesso.Text = "";
            txbDiretorio.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            btnIniciar.Enabled = true;
            btnParar.Enabled = false;
            btnReiniciar.Enabled = false;
            lblProcesso.Visible = true;
            lblDiretorio.Visible = true;
            txbProcesso.Visible = true;
            txbProcesso.Enabled = true;
            txbDiretorio.Visible = true;
            txbDiretorio.Enabled = true;
            btnDiretorio.Visible = true;
            btnDiretorio.Enabled = true;
            txbLog.Visible = false;
            
        }

        private void TxbProcesso_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (txbProcesso.Text.Length > 0))
                BtnIniciar_Click(null, null);
        }
    }
}
