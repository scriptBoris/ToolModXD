using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib
{
    public delegate void InjectorMsgHandler(string msg);

    public interface IVersionInjector
    {
        event InjectorMsgHandler EventMessanger;

        void Read(string filePath);

        void Objectivation(bool IsLoadGameplayData);

        void LoadTarget(string filePath);

        void Inject();

        void SaveResult(string dirPath);
    }
}
