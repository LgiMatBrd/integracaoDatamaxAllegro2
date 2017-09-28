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
    private Button button3;

    string stringConexao = "SERVER=191.232.241.36;" + "DATABASE=app_hml;" + "UID=root;" + "PASSWORD=vnj!db2017!;";

    public frmPrincipal()
        {
            this.InitializeComponent();
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

    private List<frmPrincipal.ImpIngresso> BuscarImpressos()
    {
        // BUSCA INGRESSOS
        string str = "select evento.titulo as Titulo, ingresso.titulo as Ingresso, coalesce(ingressocategoria.preco,0) as Valor, acesso.acesso_id as Numero, campeonato.nrApoliceSeg as Apolice, campeonato.nomeSeguradora as Seguradora, acesso.chaveAcesso as Barcode, arena_tipoingresso.titulo as legenda, case when coalesce(ingressocategoria.preco, 0) = 0 then 'VENDA PROIBIDA' ELSE ' ' END AS Observacao, coalesce(arena_setor.nome,'') as Setor, coalesce(arena_fila.identificador,'') as Fila, coalesce(arena_cadeira.identificador,'') Cadeira, concat(evento.data, ' ', evento.horario) as DataHora, categoria.titulo, acesso.acesso_id from acesso left join evento on evento.evento_id = acesso.evento_id inner join arena_tipoingresso on arena_tipoingresso.tpingresso_id = acesso.tpingresso_id inner join ingressoloteitem on ingressoloteitem.ingressoloteitem_id = acesso.acesso_id inner join ingressocategoria on ingressocategoria.ingressocategoria_id = ingressoloteitem.chave and ingressoloteitem.tipo = 'categoria' inner join ingresso on ingresso.ingresso_id = ingressocategoria.ingresso_id inner join campeonato on campeonato.campeonato_id = evento.campeonato_id inner join categoria on categoria.categoria_id = ingressocategoria.categoria_id left join arena_cadeira on arena_cadeira.cadeira_id = acesso.cadeira_id left join arena_fila on arena_fila.fila_id = arena_cadeira.fila_id left join arena_setor on arena_setor.setor_id = arena_fila.setor_id where acesso.`status` = 9";

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

            Console.WriteLine("zika");
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
            // MIOLO 
            /////////////////
            string canhoto = "<STX>L<CR>D11" +
                "<CR>29000030" + Centralizar2(125, ing.Titulo) + "0390" + ing.Titulo +
                "<CR>29000020" + Centralizar2(110, ing.Legenda) + "0370" + ing.Legenda +
                "<CR>29000030" + Centralizar2(115, ing.Observacao) + "0350" + ing.Observacao +
                "<CR>290000301630335" + ing.DataHora +
                "<CR>29000060" + Centralizar2(135, ing.Valor.ToString()) + "0300" + "R$ " + ing.Valor +
                "<CR>29000020" + Centralizar2(100, ing.Seguradora) + "0290" + ing.Seguradora +
                "<CR>29000020" + Centralizar2(100,ing.Apolice) + "0278" + ing.Apolice +
                "<CR>";
             
            /////////////////
            // CORPO DO INGRESSO
            /////////////////
            string corpoIngresso = "<CR>" +
                "<CR>390000400400" + Centralizar2(180, ing.Titulo) + "" + ing.Titulo +
                "<CR>390000200600190" + ing.Observacao +
                "<CR>390000300800180" + ing.Legenda +
                "<CR>390000201000235" + "Data: " + ing.DataHora +
                "<CR>390000201200235" + "Valor:  R$ " + ing.Valor +
                "<CR>390000101450235" + ing.Seguradora + "  " +ing.Apolice +
                "<CR>";

            // CÓDIGO DE BARRAS IMBUTIDO ABAIXO
            PrinterSettings settings = new PrinterSettings();
            RawPrinterHelper.SendStringToPrinter(settings.PrinterName.ToString(), (canhoto + corpoIngresso  + "3d9404001900240" + ing.Barcode + "<CR>" + "Q0001<CR>" + "E<CR>").Replace("<SOH>", "\x0001").ToString().Trim().Replace("<STX>", "\x0002").ToString().Trim().Replace("<CR>", "\r\n").ToString().Trim());
        }

        private void button1_Click(object sender, EventArgs e)
    {
      this.ImprimirIng(new frmPrincipal.ImpIngresso()
      {
        Titulo = "JUVENTUDE x PAYSANDU",
        Ingresso = "Arquibancada Descoberta",
        Valor = 100.00,
        Numero = 89257,
        Seguradora = "Itau Seguros",
        Apolice = "00.82.00595970052",
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

    private int Centralizar2(int iPosIni, string sTexto)
    {
        for (int index = 0; index < sTexto.Length; ++index)
        {
            iPosIni = iPosIni + 3;
        }
        Console.WriteLine(iPosIni);
        return iPosIni;
    }

        private void timer1_Tick(object sender, EventArgs e)
    {
      this.timer1.Enabled = false;
      try
      {
                //string CAIXA = this.comboBox1.SelectedValue.ToString();
                this.BuscarImpressos();
      }
      finally
      {
                this.timer1.Enabled = true;
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
                this.timer1.Enabled = true;
                this.button2.Enabled = false;
            this.button3.Enabled = true;
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
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 155);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(230, 50);
            this.button1.TabIndex = 0;
            this.button1.Text = "Testar Impressão";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(27, 43);
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
            this.button3.Location = new System.Drawing.Point(27, 99);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(230, 50);
            this.button3.TabIndex = 2;
            this.button3.Text = "Parar Monitoramento";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "frmPrincipal";
            this.Text = "Integrador de Impressões";
            this.ResumeLayout(false);

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
