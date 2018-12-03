using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore
{
    //[Serializable]
    //public delegate void FileRecivedHdlr(object sender, RmtFile e);
    //[ComVisible(true)]
    //public class RemoteObject : MarshalByRefObject
    //{
    //    public event FileRecivedHdlr FileRecved;
        
    //    private void SendObj([MarshalAs(UnmanagedType.AsAny)]RmtFile obj)
    //    {
    //        FileRecved.Invoke(this, obj);
    //    }
    //    public void SendObj([MarshalAs(UnmanagedType.LPWStr)]string path, 
    //                        [MarshalAs(UnmanagedType.Bool)]bool isUpdate, 
    //                        [MarshalAs(UnmanagedType.LPWStr)]string oldFather,
    //                        [MarshalAs(UnmanagedType.Bool)]bool processNow)
    //    {
    //        var obj = new RmtFile(path, isUpdate, oldFather, processNow);
    //        FileRecved.Invoke(this, obj);
    //    }
    //    public void SendObj([MarshalAs(UnmanagedType.LPWStr)]string path)
    //    {
    //        var obj = new RmtFile(path);
    //        FileRecved.Invoke(this, obj);
    //    }
    //    public void SendObj([MarshalAs(UnmanagedType.LPWStr)]string path,
    //                        [MarshalAs(UnmanagedType.Bool)]bool processNow)
    //    {
    //        var obj = new RmtFile(path, processNow);
    //        FileRecved.Invoke(this, obj);
    //    }

    //    /// <summary>
    //    /// 自動的に切断されるのを回避する
    //    /// </summary>
    //    public override object InitializeLifetimeService()
    //    {
    //        return null;
    //    }
    //}
}
