using ReaLTaiizor.Controls;
using ReaLTaiizor.Forms;
using System.Drawing.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AcademicoAppV1
{
    public partial class FormAluno : MaterialForm
    {

        #region Variáveis

        string alunosFileName = "alunos.txt";
        bool isEditMode = false;
        int indexSelecionado = 0;

        #endregion

        #region Métodos
        public FormAluno()
        {
            InitializeComponent();
        }

        private void LimpaCampos()
        {
            isEditMode = false;
            foreach (var control in tabPageCadastro.Controls)
            {
                if (control is MaterialTextBoxEdit textBox)
                    textBox.Clear();
                if (control is MaterialMaskedTextBox maskedTextBox)
                    maskedTextBox.Clear();
            }
        }

        private void Salvar()
        {
            var line = $"{txtMatricula.Text};" +
                $"{txtDataNascimento.Text};" +
                $"{txtNome.Text};" +
                $"{txtEndereco.Text};" +
                $"{txtBairro.Text};" +
                $"{txtCidade.Text};" +
                $"{txtEstado.Text};" +
                $"{txtSenha.Text};";


            if (!isEditMode)
            {
                using (StreamWriter sw = new StreamWriter(alunosFileName, true))
                {
                    sw.WriteLine(line);
                }
            }
            else
            {
                var fileLines = File.ReadAllLines(alunosFileName).ToList();
                fileLines[indexSelecionado] = line;
                File.WriteAllLines(alunosFileName, fileLines);
            }
        }

        private bool ValidarFormulario()
        {
            var erro = "";
            if (string.IsNullOrEmpty(txtMatricula.Text))
            {
                erro += "Matricula deve ser informada!\n";
            }

            if (string.IsNullOrEmpty(txtNome.Text))
            {
                erro += "Nome deve ser informado!\n";
            }

            if (string.IsNullOrEmpty(txtDataNascimento.Text))
            {
                erro += "Data de deve ser informada!\n";
            }

            if (!DateTime.TryParse(txtDataNascimento.Text, out _))
            {
                erro += "Data de Nascimento Inválida!\n";
            }

            if (string.IsNullOrEmpty(txtEndereco.Text))
            {
                erro += "Data de deve ser informado!\n";
            }

            if (string.IsNullOrEmpty(txtBairro.Text))
            {
                erro += "Bairro de deve ser informado!\n";
            }

            if (string.IsNullOrEmpty(txtCidade.Text))
            {
                erro += "Cidade de deve ser informada!\n";
            }

            if (string.IsNullOrEmpty(txtSenha.Text))
            {
                erro += "Senha de deve ser informada!\n";
            }

            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show(erro, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void CarregaListView()
        {
            Cursor.Current = Cursors.WaitCursor;
            listViewConsulta.Columns.Clear();
            listViewConsulta.Items.Clear();
            listViewConsulta.Columns.Add("Matricula");
            listViewConsulta.Columns.Add("Data Nasc.");
            listViewConsulta.Columns.Add("Nome");
            listViewConsulta.Columns.Add("Endereco");
            listViewConsulta.Columns.Add("Bairro");
            listViewConsulta.Columns.Add("Cidade");
            listViewConsulta.Columns.Add("Estado");
            listViewConsulta.Columns.Add("Senha");
            var fileLines = File.ReadAllLines(alunosFileName);

            foreach (var line in fileLines)
            {
                var campos = line.Split(";");
                listViewConsulta.Items.Add(new ListViewItem(campos));
            }
            listViewConsulta.View = View.Details;
            Cursor.Current = Cursors.Default;
        }

        private void Editar()
        {
            if (listViewConsulta.SelectedItems.Count > 0)
            {
                indexSelecionado = listViewConsulta.SelectedItems[0].Index;
                var item = listViewConsulta.SelectedItems[0];
                txtMatricula.Text = item.SubItems[0].Text;
                txtDataNascimento.Text = item.SubItems[1].Text;
                txtNome.Text = item.SubItems[2].Text;
                txtEndereco.Text = item.SubItems[3].Text;
                txtBairro.Text = item.SubItems[4].Text;
                txtCidade.Text = item.SubItems[5].Text;
                txtEstado.Text = item.SubItems[6].Text;
                txtSenha.Text = item.SubItems[7].Text;
                tabControlCadastro.SelectedIndex = 0;
                txtMatricula.Focus();
            }

            else
            {

                MessageBox.Show("Selecione algum registro!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void Deletar()
        {
            if (listViewConsulta.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Deseja realmente deletar?", "Pergunta",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    indexSelecionado = listViewConsulta.SelectedItems[0].Index;
                    var fileLines = File.ReadAllLines(alunosFileName).ToList();
                    fileLines.RemoveAt(indexSelecionado);
                    File.WriteAllLines(alunosFileName, fileLines);
                }
            }
            else
            {
                MessageBox.Show("Selecione algum registro!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Eventos
        private void btnNovo_Click(object sender, EventArgs e)
        {
            LimpaCampos();
            // Mudando para a página cadastro
            tabControlCadastro.SelectedIndex = 0;
            // Campo matricula recere o foco do teclado
            txtMatricula.Focus();
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Informações não salvas serão perdidas!\n" +
                "Deseja realmente cancelar?", "Atenção",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LimpaCampos();
                // Mudando para a página consulta
                tabControlCadastro.SelectedIndex = 1;
            }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarFormulario())
            {
                Salvar();
                LimpaCampos();
                tabControlCadastro.SelectedIndex = 1;
            }
        }

        private void tabPageConsulta_Enter(object sender, EventArgs e)
        {
            CarregaListView();
        }

        private void btnEditar(object sender, EventArgs e)
        {
            Editar();
        }

        private void listViewConsulta_MousseDoubleClick(object sender, MouseEventArgs e)
        {
            Editar();
        }

        private void btnExcluir(object sender, EventArgs e)
        {
            Deletar();
            CarregaListView();
        }
        #endregion
    }
}