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
using System.Windows.Shapes;
using Obuw12.Modeli;
using System.Data.Entity;

namespace Obuw12
{
    /// <summary>
    /// Логика взаимодействия для OknoZakazi.xaml
    /// </summary>
    public partial class OknoZakazi : Window
    {
        ObuwKontext _db;
        Polzovatel _pol;
        public OknoZakazi(Polzovatel pol,ObuwKontext db)
        {
            InitializeComponent();
            _db = db;
            _pol = pol;

            if(_pol.RolId ==2)
            {
                BthDobavit.Visibility=Visibility.Collapsed;
                BthRedakt.Visibility = Visibility.Collapsed;
                BthUdalit.Visibility = Visibility.Collapsed;
            }

            Zagruzka();

        }

        void Zagruzka()
        {
            LvElement.ItemsSource= _db.Zakazi
                .Include(z => z.StatusZakaza)
                .Include(z => z.PunktVidachi)
                .Include(z => z.Polzovatel)
                .ToList();
        }

        private void Dobavit(object sender, RoutedEventArgs e)
        {
            new RedaktZakazi(null,_db).ShowDialog();
            Zagruzka();
        }

        private void Redakt(object sender, RoutedEventArgs e)
        {
            var z = LvElement.SelectedItem as Zakaz;
            if (z == null) { MessageBox.Show("Выбирите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            new RedaktZakazi(z, _db).ShowDialog();
            Zagruzka();
        }

        private void Udalit(object sender, RoutedEventArgs e)
        {
            var z = LvElement.SelectedItem as Zakaz;
            if (z == null) { MessageBox.Show("Выбирите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (_db.ZakaziTovarov.Any(zt => zt.ZakazId == z.Id)) { MessageBox.Show("Товар присутсвует в заказх", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (MessageBox.Show("Удалить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            _db.Zakazi.Remove(z);
            _db.SaveChanges();
            Zagruzka();
        }

        private void ClickLvElement(object sender, MouseButtonEventArgs e)
        {
            if (_pol.RolId == 2) return;

            var z = LvElement.SelectedItem is Zakaz;
            if (z == null) return;
            new RedaktZakazi(LvElement.SelectedItem as Zakaz, _db).ShowDialog() ;
        }

        private void BthNazad(object sender, RoutedEventArgs e)
        {
            new MainWindow(_pol).Show();
            Close();
        }
    }
}
