using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;

namespace Villeon.Helper
{
    public class FPS
    {
        private GameWindow _gameWindow;
        private float _timer = 0;
        private float _fps = 1;
        private float _max = 0;
        private float _min = 10000;
        private float _avg = 1;
        private Queue<float> _times;

        public FPS(GameWindow window)
        {
            _gameWindow = window;
            _times = new Queue<float>();
        }

        public void SetFps(float time)
        {
            _timer += time;

            if (_timer > 0.1f)
            {
                _fps = 1 / time;
                if (_fps > _max)
                    _max = _fps;
                if (_fps < _min)
                    _min = _fps;

                if (_times.Count > 100)
                    _times.Dequeue();
                _times.Enqueue(_fps);

                _avg = _times.Sum() / _times.Count;
                _gameWindow.Title = "FPS: [" + ((int)_fps) + "] Lowest: [" + ((int)_min) + "] AVG: [" + ((int)_avg) + "] Highest: [" + ((int)_max) + "]";
                _timer = 0;
            }
        }
    }
}
