using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Core
{
    public interface IApplicationCommands
    {
        CompositeCommand WindowClosingCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand WindowClosingCommand { get; } = new CompositeCommand();
    }
}
