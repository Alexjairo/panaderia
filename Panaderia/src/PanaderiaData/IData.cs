namespace PanaderiaD;
using System.Collections.Generic;



    public interface IData<T>
    {
         public void save(List<T> datos);
    public List<T> read();
}
    

