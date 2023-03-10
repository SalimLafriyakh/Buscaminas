using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Data;
using System.Collections;
using System.ComponentModel;

namespace Buscaminas
{
    class Juego : INotifyPropertyChanged
    {
        #region Atributos

        int numMinas = 10;
        int numDificultad = 10;
        int contador;
        List<Celda> celdas;
        #endregion

        #region Constructor

        public Juego()
        {

            contador = numMinas;
        }
        #endregion
        
        #region Get y Set

        public int NumMinas
        {
            get { return numMinas; }
            set
            {
                numMinas = value;
                RaisePropertyChanged("NumMinas");
            }
        }
       

        public int NumDificultad
        {
            get { return numDificultad; }
            set
            {
                numDificultad = value;
                RaisePropertyChanged("NumDificultad");
            }
        }

        //Contador
        public int Contador
        {
            get { return contador; }
            set
            {
                contador = value;
                RaisePropertyChanged("Contador");
            }
        }


        public List<Celda> Celdas
        {
            get { return celdas; }
            set
            {
                celdas = value;
                RaisePropertyChanged("Celdas");
            }
        }
        #endregion

        #region ClickCommand

        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => MyAction(), CanExecuteAction()));
            }
        }

        private bool CanExecuteAction()
        {
            return true;
        }

        public void MyAction()
        {
            NuevoJuego();
        }
        #endregion

        #region LeftClickCommand
        private ICommand _leftClickCommand;
        public ICommand LeftClickCommand
        {
            get
            {
                return _leftClickCommand ?? (_leftClickCommand = new CommandHandler_Celda((Celda c) => MyActionLeftClick(c), true));
            }
        }

        public void MyActionLeftClick(Celda c)
        {
            if (c.Mina)
            {
                c.Text = "M";
                c.ShowBomb = Visibility.Visible;
                MessageBox.Show("¡HAS PERDIDO!, JUEGO TERMINADO");
                NuevoJuego();
            }
            else
            {
                int n = GetNumMinasAlrededorCelda(c.Row, c.Column);
                c.Text = n.ToString();
                if (n == 0)
                {
                    DespejarCeldasAlrededor(c.Row, c.Column);
                }

                if (GetNumCeldasSinAbrir() == NumMinas)
                {
                    MessageBox.Show("¡HAS GANADO!");
                    NuevoJuego();
                }
            }
        }
        #endregion

        #region RightClickCommand
        private ICommand _rightClickCommand;
        public ICommand RightClickCommand
        {
            get
            {
                return _rightClickCommand ?? (_rightClickCommand = new CommandHandler_Celda((Celda c) => MyActionRightClick(c), true));
            }
        }

        public void MyActionRightClick(Celda c)
        {

            

            if (c.ShowFlag == Visibility.Hidden && c.ShowQuestion == Visibility.Hidden)
            {
                c.ShowFlag = Visibility.Visible;
                Contador--;
            }
            else if (c.ShowFlag == Visibility.Visible)
            {
                c.ShowFlag = Visibility.Hidden;
                Contador++;
                c.ShowQuestion = Visibility.Visible;
            }
            else if (c.ShowQuestion == Visibility.Visible)
            {
                c.ShowQuestion = Visibility.Hidden;
            }
        }
        #endregion

        #region Metodos
        private void NuevoJuego()
        {
            contador = numMinas;
            List<Celda> lstCeldas = new List<Celda>();
            for (int i = 0; i < NumDificultad; i++)
                for (int j = 0; j < NumDificultad; j++)
                {
                    Celda c = new Celda();
                    c.Row = i;
                    c.Column = j;
                    c.Text = "";
                    c.Mina = false;
                    lstCeldas.Add(c);
                }

            Random r = new Random();
            for (int i = 0; i < NumMinas; i++)
            {
                int x = r.Next(0, lstCeldas.Count);
                if (lstCeldas[x].Mina)
                    i--;
                lstCeldas[x].Mina = true;
            }

            Celdas = lstCeldas;
        }
               

        public int GetNumCeldasSinAbrir()
        {
            int n = 0;
            for (int i = 0; i < Celdas.Count; i++)
                if (Celdas[i].Text == string.Empty)
                    n++;
            return n;
        }

        private void DespejarCeldasAlrededor(int i, int j)
        {
            if (i >= 0 && i < numDificultad && j >= 0 && j < numDificultad)
            {
                DespejarCeldasAlrededor_Pos(i - 1, j - 1);
                DespejarCeldasAlrededor_Pos(i - 1, j);
                DespejarCeldasAlrededor_Pos(i - 1, j + 1);
                DespejarCeldasAlrededor_Pos(i, j - 1);
                DespejarCeldasAlrededor_Pos(i, j + 1);
                DespejarCeldasAlrededor_Pos(i + 1, j - 1);
                DespejarCeldasAlrededor_Pos(i + 1, j);
                DespejarCeldasAlrededor_Pos(i + 1, j + 1);
            }
        }

        private void DespejarCeldasAlrededor_Pos(int a, int b)
        {
            int n1 = GetNumMinasAlrededorCelda(a, b);

            if (a >= 0 && a < numDificultad && b >= 0 && b < numDificultad && (GetCelda(a, b).Text == string.Empty))
            {
                GetCelda(a, b).Text = n1.ToString();

                if (n1 == 0)
                {
                    DespejarCeldasAlrededor(a, b);
                }
            }


        }

        private int GetNumMinasAlrededorCelda(int i, int j)
        {
            bool b1 = ExisteMinaEnCelda(i - 1, j - 1);
            bool b2 = ExisteMinaEnCelda(i - 1, j);
            bool b3 = ExisteMinaEnCelda(i - 1, j + 1);
            bool b4 = ExisteMinaEnCelda(i, j - 1);
            bool b5 = ExisteMinaEnCelda(i, j + 1);
            bool b6 = ExisteMinaEnCelda(i + 1, j - 1);
            bool b7 = ExisteMinaEnCelda(i + 1, j);
            bool b8 = ExisteMinaEnCelda(i + 1, j + 1);

            int n = 0;
            if (b1) n++;
            if (b2) n++;
            if (b3) n++;
            if (b4) n++;
            if (b5) n++;
            if (b6) n++;
            if (b7) n++;
            if (b8) n++;
            return n;
        }


        private bool ExisteMinaEnCelda(int i, int j)
        {
            if (i >= 0 && i < numDificultad && j >= 0 && j < numDificultad)
                return GetCelda(i, j).Mina;
            return false;
        }

        private Celda GetCelda(int row, int column)
        {
            return celdas.Where(e => e.Row == row && e.Column == column).First();
        }
        #endregion
              

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class CommandHandler_Celda : ICommand
    {
        private Action<Celda> _action;
        private bool _canExecute;
        public CommandHandler_Celda(Action<Celda> action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action((Celda)parameter);
        }
    }
}
