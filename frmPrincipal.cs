using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace VNJIngressos
{
  public class frmPrincipal : Form
  {
    private IContainer components = (IContainer) null;
    private Button button1;
    private Timer timer1;
    private Button button2;
    private ComboBox comboBox1;
    private Label label1;
    private Button button3;

    string stringConexao = "SERVER=localhost;" + "DATABASE=vnj_local_novo;" + "UID=root;" + "PASSWORD=afornalli;";

    public frmPrincipal()
        {
            this.InitializeComponent();
            // Popula o ComboBox
            this.comboBox1.Items.Insert(0, "Selecione uma opção");
            this.comboBox1.SelectedIndex = 0;
            MySqlConnection connection = new MySqlConnection(stringConexao);
            string command = "SELECT caixa_id,nome FROM caixa;";
            MySqlDataAdapter da = new MySqlDataAdapter(command, connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                // SÓ FUNCIONA ESSA LINHA SE A TABELA ESTIVER NA ODERM!
                //comboBox1.Items.Insert(Convert.ToInt16(row.ItemArray[0]), row.ItemArray[0].ToString());

                // CASO NÃO ESTEJA NA ORDEM UTILIZAR ESSE CÓDIGO!
                string rowz = string.Format("{0} - {1}", row.ItemArray[0], row.ItemArray[1]);
                this.comboBox1.Items.Add(rowz);
            }
            connection.Close();
    }

    private void updateAcesso(frmPrincipal.ImpIngresso ing, MySqlConnection conn)
    {
      string str = "update acesso set status = 0 where acesso_id = " + ing.acesso_id.ToString();
      MySqlCommand mySqlCommand = new MySqlCommand();
      MySqlConnection mySqlConnection = new MySqlConnection(stringConexao);
      mySqlConnection.Open();
      try
      {
        mySqlCommand.Connection = mySqlConnection;
        mySqlCommand.CommandText = str;
        mySqlCommand.CommandType = CommandType.Text;
        mySqlCommand.ExecuteNonQuery();
      }
      finally
      {
        mySqlCommand.Dispose();
        mySqlConnection.Close();
      }
    }

    private List<frmPrincipal.ImpIngresso> BuscarImpressos(string CAIXA)
    {
        // BUSCA INGRESSOS
        string str = "select evento.titulo as Titulo, " + "           ingresso.titulo as Ingresso, " + "           coalesce(ingressocategoria.preco,0) as Valor, " + "           acesso.acesso_id as Numero, " + "           campeonato.nrApoliceSeg as Apolice," + "           campeonato.nomeSeguradora as Seguradora, " + "           acesso.chaveAcesso as Barcode, " + "           arena_tipoingresso.titulo as legenda,   " + "           case when coalesce(ingressocategoria.preco, 0) = 0 then 'VENDA PROIBIDA' ELSE ' ' END AS Observacao, " + "           coalesce(arena_setor.nome,'') as Setor, " + "           coalesce(arena_fila.identificador,'') as Fila, " + "           coalesce(arena_cadeira.identificador,'') Cadeira, " + "           concat(evento.data, ' ', evento.horario) as DataHora, " + "           categoria.titulo, " + "           acesso.acesso_id " + "from acesso " + "left join evento                on evento.evento_id = acesso.evento_id " + "inner join arena_tipoingresso on arena_tipoingresso.tpingresso_id = acesso.tpingresso_id " + "inner join ingressocategoria on ingressocategoria.ingressocategoria_id = acesso.ingressocategoria_id" + " inner join ingresso on ingresso.ingresso_id = ingressocategoria.ingresso_id " + "inner join campeonato on campeonato.campeonato_id = evento.campeonato_id " + "inner join categoria on categoria.categoria_id = ingressocategoria.categoria_id " + "left join arena_cadeira on arena_cadeira.cadeira_id = acesso.cadeira_id " + "left join arena_fila on arena_fila.fila_id = arena_cadeira.fila_id " + "left join arena_setor on arena_setor.setor_id = arena_fila.setor_id " + "where acesso.status = 11 and acesso.caixa_id = " + CAIXA;
            
        List<frmPrincipal.ImpIngresso> impIngressoList = new List<frmPrincipal.ImpIngresso>();
            
        MySqlConnection conn = new MySqlConnection(stringConexao);
        conn.Open();
        try
        {
        MySqlCommand mySqlCommand = new MySqlCommand();
        mySqlCommand.Connection = conn;
        mySqlCommand.CommandText = str;
        mySqlCommand.CommandType = CommandType.Text;
        MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
        while (mySqlDataReader.Read())
        {
            frmPrincipal.ImpIngresso ing = new frmPrincipal.ImpIngresso();
            ing.Titulo = mySqlDataReader.GetString(0).RemoveAccents();
            ing.Ingresso = mySqlDataReader.GetString(1).RemoveAccents();
            ing.Valor = mySqlDataReader.GetDouble(2);
            ing.Numero = mySqlDataReader.GetInt32(3);
            ing.Apolice = mySqlDataReader.GetString(4).RemoveAccents();
            ing.Seguradora = mySqlDataReader.GetString(5).RemoveAccents();
            ing.Barcode = mySqlDataReader.GetString(6).RemoveAccents();
            ing.Legenda = mySqlDataReader.GetString(7).RemoveAccents();
            ing.Observacao = mySqlDataReader.GetString(8).RemoveAccents();
            ing.Setor = mySqlDataReader.GetString(9).RemoveAccents();
            ing.Fila = mySqlDataReader.GetString(10).RemoveAccents();
            ing.Cadeira = mySqlDataReader.GetString(11).RemoveAccents();
            mySqlDataReader.GetString(12).RemoveAccents();
            ing.DataHora = DateTime.ParseExact(mySqlDataReader.GetString(12), "yyyy-MM-dd HH:mm", (IFormatProvider) CultureInfo.InvariantCulture);
            ing.Categoria = mySqlDataReader.GetString(13).RemoveAccents();
            ing.acesso_id = mySqlDataReader.GetInt32(14);
            impIngressoList.Add(ing);
            this.updateAcesso(ing, conn);
            this.ImprimirIng(ing);
        }
        }
        finally
        {
        conn.Close();
        }
    return impIngressoList;
    }

    private void ImprimirIng(frmPrincipal.ImpIngresso ing)
        {

            /////////////////
            // MIOLO DO CANHOTO
            /////////////////

            string str1 = "<STX>m<CR>";
            str1 = "<STX>O0220<CR>";
            str1 = "<STX>V1<CR>";
            str1 = "<STX>f440<CR>";
            string str2 = "<STX>L<CR>";
            int num1 = 14;
            if (ing.Titulo.Length > 18)
                num1 = 12;
            if (ing.Titulo.Length > 22)
                num1 = 10;
            
            // TAMANHO DO INGRESSO
            string str3 = str2 + "D10<CR>";
            // TITULO CANHOTO
            string str4 = ing.Titulo.Length > 14 ? ing.Titulo.Substring(0, 14) : ing.Titulo;
            // VALOR E SEGURO CANHOTO
            string str5 = str3 + "290000301900390" + str4.ToUpper() + "<CR>" + "290000201600370" + (ing.Ingresso.Length > 24 ? ing.Ingresso.Substring(0, 24) : ing.Ingresso) + "<CR>" + "4911A0800250065Valor: " + string.Format("{0:C}", (object)ing.Valor) + "<CR>" + "4911A0801150065Num:<CR>";
            string str6 = "4911A0801600065";
            int numero = ing.Numero;
            string str7 = numero.ToString().PadLeft(6, '0');
            string str8 = "<CR>";
            string str9 = str5 + str6 + str7 + str8 + "4911A0800250085" + ing.Seguradora + "<CR>" + "4911A0800250105Num. ApoliceLGI " + ing.Apolice + "<CR>";
            
            string canhoto = "<STX>L<CR>D11" +
                "<CR>290000301900390JEC x BRASIL DE PELOTAS" +
                "<CR>290000201600370Arquibancada Descoberta" +
                "<CR>290000301600350VENDA PROIBIDA" +
                "<CR>290000301550335 14/04/1997 18:06" +
                "<CR>290000601500300R$ 0,00" +
                "<CR>290000201600290Seguradora Itau Seguros" +
                "<CR>290000201400280Num. 132465789" +
                "<CR>";

            /////////////////
            // MIOLO DO MEIO
            /////////////////
            int num2 = 130;
            string str10 = ing.Titulo.Length > 28 ? ing.Titulo.Substring(0, 28) : ing.Titulo;
            string str11 = canhoto + "1911A" + num1.ToString() + num2.ToString().PadLeft(4, '0') + "0160" + str10.ToUpper() + "<CR>";
            num2 -= 11;
            string str12 = str11 + this.Centralizar(ing.Legenda.ToUpper(), 10, "1911A08" + num2.ToString().PadLeft(4, '0'), 160, 220);
            num2 -= 15;
            string str14 = str12 + this.Centralizar(ing.Observacao.ToUpper(), 10, "1911A10" + num2.ToString().PadLeft(4, '0'), 160, 220);
            //num2 -= 15;
            //string str14 = str13 + "1911A09" + num2.ToString().PadLeft(4, '0') + "0160Setor:<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0190" + ing.Setor + "<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0270Fila:<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0300" + ing.Fila + "<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0330Cadeira:<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0375" + ing.Cadeira + "<CR>";
            num2 -= 15;
            string str15 = str14 + "1911A09" + num2.ToString().PadLeft(4, '0') + "0160DataLGI:<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0190" + string.Format("{0:dd/MM/yyyy}", (object)ing.DataHora) + "<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0270HoraLGI:<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0300" + string.Format("{0:HH:mm}", (object)ing.DataHora) + "<CR>";
            num2 -= 15;
            string[] strArray = new string[6]
            {
        str15 + "1911A09" + num2.ToString().PadLeft(4, '0') + "0160ValorLGI:<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0190" + string.Format("{0:C}", (object) ing.Valor) + "<CR>" + "1911A09" + num2.ToString().PadLeft(4, '0') + "0270Num.LGI:<CR>",
        "1911A09",
        num2.ToString().PadLeft(4, '0'), 
        "0300",
        null,
        null
            };
            int index1 = 4;
            numero = ing.Numero;
            string str16 = numero.ToString().PadLeft(8, '0');
            strArray[index1] = str16;
            int index2 = 5;
            string str17 = "<CR>";
            strArray[index2] = str17;
            string str18 = string.Concat(strArray);
            num2 -= 15;
            string str19 = str18 + "1911A08" + num2.ToString().PadLeft(4, '0') + "0160" + ing.Seguradora + "<CR>";
            num2 -= 15;
            string str20 = str19 + "1911A08" + num2.ToString().PadLeft(4, '0') + "0160Num. ApoliceLGI " + ing.Apolice + "<CR>";
            num2 -= 65;


            // CÓDIGO DE BARRAS IMBUTIDO ABAIXO
            PrinterSettings settings = new PrinterSettings();
            RawPrinterHelper.SendStringToPrinter(settings.PrinterName.ToString(), (str20 + this.Centralizar(ing.Categoria.ToUpper(), 10, "1911A10" + num2.ToString().PadLeft(4, '0'), 160, 220) + "3d9404001900240" + ing.Barcode + "<CR>" + "Q0001<CR>" + "E<CR>").Replace("<SOH>", "\x0001").ToString().Trim().Replace("<STX>", "\x0002").ToString().Trim().Replace("<CR>", "\r\n").ToString().Trim());
        }

        private void button1_Click(object sender, EventArgs e)
    {
      this.ImprimirIng(new frmPrincipal.ImpIngresso()
      {
        Titulo = "LUIGI E MUITO ZIKA",
        Ingresso = "Arquibancada Descoberta",
        Valor = 2.45,
        Numero = 89257,
        Seguradora = "Seguradora Itau Seguros",
        Apolice = "Apolice: 00.82.00595970052",
        Barcode = "016143577727",
        Legenda = "CBF",
        Observacao = "VENDA PROIBIDA",
        Setor = "Camarote",
        Fila = "45",
        Cadeira = "12",
        DataHora = DateTime.Now,
        Categoria = "Inteira"
      });
    }

    private string Centralizar(string sTexto, int iEspacamento, string sPrefixo, int iPosIni, int iLargura)
    {
      string str = "";
      sTexto = sTexto.ToUpper();
      iPosIni += (iLargura - sTexto.Length * iEspacamento) / 2;
      for (int index = 0; index < sTexto.Length; ++index)
      {
        int num = iPosIni;
        if ((int) sTexto[index] == 73)
          num += iEspacamento / 3;
        str = str + sPrefixo + num.ToString().PadLeft(4, '0') + sTexto[index].ToString() + "<CR>";
        iPosIni += iEspacamento;
      }
      return str;
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      this.timer1.Enabled = false;
      try
      {
                //string CAIXA = this.comboBox1.SelectedValue.ToString();
                String stringID  = comboBox1.Text;
                string[] CAIXA = stringID.Split(" - ".ToCharArray());
                this.BuscarImpressos(CAIXA[0]);
      }
      finally
      {
                this.timer1.Enabled = true;
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
            if (this.comboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("Não é possível iniciar a venda sem informar um caixa!");
            } else
            {
                this.timer1.Enabled = true;
                this.button2.Enabled = false;
                this.button3.Enabled = true;
            }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      this.timer1.Enabled = false;
      this.button2.Enabled = true;
      this.button3.Enabled = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 199);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(230, 50);
            this.button1.TabIndex = 0;
            this.button1.Text = "Testar Impressão";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(27, 87);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(230, 50);
            this.button2.TabIndex = 1;
            this.button2.Text = "Iniciar Venda de Ingresso";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(27, 143);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(230, 50);
            this.button3.TabIndex = 2;
            this.button3.Text = "Parar Monitoramento";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(27, 48);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(230, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Selecione o seu caixa:";
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "frmPrincipal";
            this.Text = "Integrador de Impressões";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    private class ImpIngresso
    {
      public int acesso_id { get; set; }

      public string Titulo { get; set; }

      public string Ingresso { get; set; }

      public double Valor { get; set; }

      public int Numero { get; set; }

      public string Seguradora { get; set; }

      public string Apolice { get; set; }

      public string Barcode { get; set; }

      public string Legenda { get; set; }

      public string Observacao { get; set; }

      public string Setor { get; set; }

      public string Fila { get; set; }

      public string Cadeira { get; set; }

      public DateTime DataHora { get; set; }

      public string Categoria { get; set; }
    }
    
    }
}
