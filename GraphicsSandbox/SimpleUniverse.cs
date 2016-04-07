using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GraphicsSandbox
{
    public class SimpleUniverse
    {
        private readonly Dispatcher _dispatcher;

        public SimpleUniverse(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Elements = new ObservableCollection<Element>();
            Elements.Add(new Ball(50, new Dimensions(100, 0)));
            Elements.Add(new Ball(50, new Dimensions(100, 50) ));
            Elements.Add(new Ball(50, new Dimensions(100, 100) ));
            Elements.Add(new Ball(50, new Dimensions(100, 150) ));
            Elements.Add(new Ball(50, new Dimensions(100, 200) ));
            Task tasks = new Task(Action);
            tasks.Start();
        }

        private void Action()
        {
            while (true)
            {
                foreach (var element in Elements) {

                    element.Location = new Dimensions(element.Location.X, element.Location.Y + 1);
                    
                    Thread.Sleep(100);
                }   
            }
            
            
        }

        public ObservableCollection<Element> Elements {
            get;
            set;
        }
    }
}