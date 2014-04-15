using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Input;
using Tao.OpenGl;

namespace Shooter
{
    class CreditState:IGameObject
    {
        const double _timeOut = 5;
        double _countDown = _timeOut;

        StateSystem _system;
        Input _input;
        Font _generalFont;
        Font _titleFont;
        //Font _titleFont;
        PersistantGameData _gameData;
        Renderer _renderer = new Renderer();

        Text _titleHelp;
        Text _blurbHelp;

        public CreditState(PersistantGameData data, StateSystem system, Input input, Font generalFont, Font titleFont)
        {
            _gameData = data;
            _system = system;
            _input = input;
            _generalFont = generalFont;
            _titleFont = titleFont;

            _titleHelp = new Text("How to Play", _titleFont);
            _blurbHelp = new Text("", _generalFont);
            _blurbHelp = new Text("Directional keys-move,Enter or X(gamepad) to fire", _generalFont);
            

            FormatText(_titleHelp, 300);
            FormatText(_blurbHelp, 200);
        }

        private void FormatText(Text _text, int yPosition)
        {
            _text.SetPosition(-_text.Width / 2, yPosition);
            _text.SetColor(new Color(0, 0, 0, 1));

        }
        #region IGameObject Members
        
        public void Update(double elapsedTime)
        {
            _countDown -= elapsedTime;

            if (_countDown <= 0 ||
                    _input.Controller.ButtonA.Pressed ||
                    _input.Keyboard.IsKeyPressed(System.Windows.Forms.Keys.Enter))
            {
                Finish();
            }
        }
        private void Finish()
        {
            _gameData.JustWon = false;
            _system.ChangeState("start_menu");
            _countDown = _timeOut;
        }

        public void Render()
        {
            Gl.glClearColor(1, 1, 1, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

                _renderer.DrawText(_titleHelp);
                _renderer.DrawText(_blurbHelp);
            
            _renderer.Render();
        }

        public void Activated()
        {
        }
    }
}
        #endregion