using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentFactory
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propName = null)
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


        protected virtual bool SetValue<TKind>(ref TKind Source, TKind NewValue, [CallerMemberName] string propName = null)
        {
            Source = NewValue;
            OnPropertyChanged(propName);
            return true;
        }

        /// <summary>
        /// It using Binary Serialization.
        /// TKind must be [Serializable] = BinaryFormatter Serialization
        /// example: https://www.csharpstudy.com/Practical/Prac-Serialization.aspx
        /// </summary>
        /// <typeparam name="TKind"></typeparam>
        /// <param name="Source"></param>
        /// <param name="NewValue"></param>
        /// <param name="syncLock"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected virtual bool SetValueLockClone<TKind>(ref TKind Source, TKind NewValue, object syncLock, [CallerMemberName] string propName = null)
        {
            TKind tempValue = MySerialize.DeepClone<TKind>(NewValue);
            lock (syncLock){
                Source = tempValue;
            }            
            OnPropertyChanged(propName);
            return true;
        }

        protected virtual bool SetValueLock<TKind>(ref TKind Source, TKind NewValue, object syncLock, [CallerMemberName] string propName = null)
        {   
            lock (syncLock)
            {
                Source = NewValue;
            }
            OnPropertyChanged(propName);
            return true;
        }


        protected virtual TKind GetValueLock<TKind>(ref TKind Source, object syncLock)
        {            
            lock (syncLock)
            {
                return Source;                
            }
        }




        protected virtual bool SetValueInterlocked(ref int Source, int NewValue, [CallerMemberName] string propName = null)
        {
            Interlocked.Exchange(ref Source,  NewValue);
            OnPropertyChanged(propName);
            return true;
        }

        protected virtual bool SetValueInterlocked(ref long Source, long NewValue, [CallerMemberName] string propName = null)
        {
            Interlocked.Exchange(ref Source, NewValue);
            OnPropertyChanged(propName);
            return true;
        }

        protected virtual bool SetValueInterlocked(ref double Source, double NewValue, [CallerMemberName] string propName = null)
        {
            Interlocked.Exchange(ref Source, NewValue);
            OnPropertyChanged(propName);
            return true;
        }




        protected virtual bool SetValueInterlocked(ref int Source, int NewValue, params string[] Notify)
        {
            Interlocked.Exchange(ref Source, NewValue);
            foreach (var i in Notify)
                OnPropertyChanged(i);
            return true;
        }

        protected virtual bool SetValueInterlocked(ref long Source, long NewValue, params string[] Notify)
        {
            Interlocked.Exchange(ref Source, NewValue);
            foreach (var i in Notify)
                OnPropertyChanged(i);
            return true;
        }

        protected virtual bool SetValueInterlocked(ref double Source, double NewValue, params string[] Notify)
        {
            Interlocked.Exchange(ref Source, NewValue);
            foreach (var i in Notify)
                OnPropertyChanged(i); 
            return true;
        }


        protected virtual bool SetValue<TKind>(ref TKind Source, TKind NewValue, params string[] Notify)
        {
            //Set value if the new value is different from the old
            if (!Source.Equals(NewValue))
            {
                Source = NewValue;

                //Notify all applicable properties
                foreach (var i in Notify)
                    OnPropertyChanged(i);

                return true;
            }

            // example.
            /*
                bool someProperty = false;
                public bool SomeProperty
                {
                    get
                    {
                        return someProperty;
                    }
                    set
                    {
                        SetValue(ref someProperty, value, "SomeProperty", "AnotherProperty");
                    }
                }
             */


            return false;
        }

    }
}
