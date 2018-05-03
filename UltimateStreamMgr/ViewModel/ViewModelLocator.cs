using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Metaco.Trader"
                           x:Key="Locator" />
  </Application.Resources>

  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm

*/



namespace UltimateStreamMgr.ViewModel
{
    public class ViewModelLocator
    {
        Dictionary<Type, object> _Container;
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            _Container = new Dictionary<Type, object>();
        }

        public T Resolve<T>() where T : ViewModelBase , new()
        {
            if(_Container.ContainsKey(typeof(T)))
            {
                return (T)_Container[typeof(T)];
            }
            else
            {
                T obj = new T();
                _Container[typeof(T)] = obj;
                return obj;
            }
        }
        public object Resolve(string className)
        {
            var assembly = Assembly.GetExecutingAssembly();

            Type type = assembly.GetTypes().First(t => t.Name == className);

            MethodInfo resolver = typeof(ViewModelLocator).GetMethod("Resolve").MakeGenericMethod(type);

            return resolver.Invoke(this, null);
        }
    }
}
