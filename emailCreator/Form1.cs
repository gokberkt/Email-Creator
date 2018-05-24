using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace emailCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Email_Creator_DBEntities db = new Email_Creator_DBEntities();

        private void Form1_Load(object sender, EventArgs e)
        {
            FillComboBox();
        }

        public void FillComboBox()
        {
            cmbUzantı.Items.Add("hotmail.com");
            cmbUzantı.Items.Add("gmail.com");
            cmbUzantı.Items.Add("timurozu.com");
        }

        public static string MD5Sifrele(string password)
        {

            // MD5CryptoServiceProvider sınıfının bir örneğini oluşturduk.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //Parametre olarak gelen veriyi byte dizisine dönüştürdük.
            byte[] dizi = Encoding.UTF8.GetBytes(password);
            //dizinin hash'ini hesaplattık.
            dizi = md5.ComputeHash(dizi);
            //Hashlenmiş verileri depolamak için StringBuilder nesnesi oluşturduk.
            StringBuilder sb = new StringBuilder();
            //Her byte'i dizi içerisinden alarak string türüne dönüştürdük.

            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            //hexadecimal(onaltılık) stringi geri döndürdük.
            return sb.ToString();
        }



        public bool Validations()
        {
            bool isValidated = true;
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) || cmbUzantı.SelectedItem == null)
            {
                isValidated = false;
            }
            else
            {
                isValidated = true;
            }
            return isValidated;
        }

        public string CharacterReplacer()
        {
            string name = txtName.Text.ToLower().Replace('ı', 'i').Replace('ğ', 'g').Replace('ü', 'u').Replace('ş', 's').Replace('ö', 'o').Replace('ç', 'c');
            string lastname = txtLastName.Text.ToLower().Replace('ı', 'i').Replace('ğ', 'g').Replace('ü', 'u').Replace('ş', 's').Replace('ö', 'o').Replace('ç', 'c');

            return name + lastname;
        }
        public string CreateEmail()
        {
            string email = "";
            if (Validations())
            {
                if (cmbUzantı.SelectedIndex == 0)
                {
                    email = CharacterReplacer() + "@hotmail.com";
                }
                else if (cmbUzantı.SelectedIndex == 1)
                {
                    email = CharacterReplacer() + "@gmail.com";
                }
                else if(cmbUzantı.SelectedIndex == 2)
                {
                    email = CharacterReplacer() + "@timurozu.com";
                }
            }

            return email;
        }

        public string PasswordToMD5(string password)
        {
            string şifre = "";
            if (password != null)
            {
                şifre = MD5Sifrele(password);
            }
            else
            {
                MessageBox.Show("Şifre boş olamaz");
            }
            return şifre;
        }

        public void SaveToDatabase()
        {

            UserInfo ui = new UserInfo();
            try
            {
                if (Validations())
                {
                    ui.Name = txtName.Text;
                    ui.LastName = txtLastName.Text;
                    ui.Password = PasswordToMD5(txtPassword.Text);
                    ui.Email = CreateEmail();
                    db.UserInfo.Add(ui);
                    db.SaveChanges();
                    MessageBox.Show("Kayıt işlemi başarılı!");
                    FillDataGridView();
                    Clear();
                }
                else
                {
                    MessageBox.Show("Ad , Soyad ve ya email boş olamaz!");
                    Clear();
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Kaydederken bir sorun oluştu !");
            }

        }
        public void Clear()
        {
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    TextBox text = (TextBox)item;
                    text.Clear();
                }
            }

        }
        public void FillDataGridView()
        {
            dgvUserInfos.DataSource = db.UserInfo.ToList();
            dgvUserInfos.Columns[0].Visible = false;
            dgvUserInfos.Columns[4].Visible = false;
            dgvUserInfos.Columns[1].HeaderText = "AD";
            dgvUserInfos.Columns[2].HeaderText = "SOYAD";
            dgvUserInfos.Columns[3].HeaderText = "E-MAIL";
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            SaveToDatabase();
        }
    }
}

