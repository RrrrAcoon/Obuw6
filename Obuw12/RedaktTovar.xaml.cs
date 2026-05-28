using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Obuw12.Modeli;
using System.IO;

namespace Obuw12
{
    /// <summary>
    /// Логика взаимодействия для RedaktTovar.xaml
    /// </summary>
    public partial class RedaktTovar : Window
    {
        Tovar _tovar = new Tovar();
        ObuwKontext _db;
      
        public RedaktTovar(Tovar vibraniyTovar,ObuwKontext db)
        {
            InitializeComponent();
            _db = db;

            ComboEdinica.ItemsSource = _db.EdiniciIzmereniy.ToList();
            ComboPostavchik.ItemsSource = _db.Postavchiki.ToList();
            ComboProizvoditel.ItemsSource = _db.Proizvoditeli.ToList();
            ComboKategoriya.ItemsSource = _db.Kategorii.ToList();

            if(_tovar != null)_tovar = vibraniyTovar;
            DataContext = vibraniyTovar;
            if(_tovar.Id==0)
            {
                LbId.Visibility = Visibility.Collapsed;
                txtId.Visibility = Visibility.Collapsed;
            }


            
        }

        private void FotoClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Изображения|*.png;*.jpg;*.jpeg" };
            if (dlg.ShowDialog() != true) return;

            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.CreateOptions=BitmapCreateOptions.IgnoreImageCache;
            img.UriSource = new Uri(dlg.FileName);
            img.EndInit();
            if(img.PixelWidth>300 || img.PixelHeight>200)
            {
                MessageBox.Show("Картинка не может быть более 300х200 пикселей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string papka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Kartinki");
            string fileImya = Path.GetFileName(dlg.FileName);

            if (!string.IsNullOrEmpty(_tovar.Foto) && _tovar.Foto != fileImya && _tovar.Foto != "picture.png")
            {
                string old = Path.Combine(papka, _tovar.Foto);
                if (File.Exists(old)) File.Delete(old);
            }
            File.Copy(dlg.FileName, Path.Combine(papka, fileImya), true);
            _tovar.Foto = fileImya;
            MessageBox.Show("Фото сохранено", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

        private void Sohranit(object sender, RoutedEventArgs e)
        {
            if (_tovar.EdinicaIzmereniyaId == 0 || _tovar.KategoriyaId== 0 || _tovar.PostavchikId== 0|| _tovar.ProizvoditelId == 0)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_tovar.Cena <0 ||_tovar.Kolichestvo<0)
            {
                MessageBox.Show("количество и цена не могут быть отрицательными", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_tovar.Id == 0) _db.Tovari.Add(_tovar);
            _db.SaveChanges();
            MessageBox.Show("Сохранено", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
