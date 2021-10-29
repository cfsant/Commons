using Commons.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Commons.Utils
{
    public static class Mapper
    {
        public static T ToAssign<S, T>(S source, T target)
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                ToAssign<S, T>(prop, source, target);
            }

            return target;
        }

        public static object Update(object source, object target, Type type)
        {
            var result = SoftCopy(target, type);

            if (!typeof(IBase).IsAssignableFrom(type))
            {
                throw new Exception("The 'Update' method is only applicable to types derived from 'IBase'.");
            }

            if (source.GetType() != type)
            {
                throw new Exception("Input type of parameter 'source' is invalid.");
            }

            if (target.GetType() != type)
            {
                throw new Exception("Input type of parameter 'target' is invalid.");
            }

            foreach (PropertyInfo prop in type.GetProperties())
            {
                if ("Id".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("OwnerId".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("PublisherId".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("Insertion".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("Change".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if (prop.GetMethod == null || prop.GetMethod == default)
                {
                    continue;
                }

                if (prop.SetMethod == null || prop.SetMethod == default)
                {
                    continue;
                }

                prop.SetValue(result, prop.GetValue(source));
            }

            return result;
        }

        public static T Update<T>(T source, T target)
        {
            var type = typeof(T);
            var properties = type.GetRuntimeProperties();
            if (properties == null || properties == default || properties.Count() < 1)
            {
                throw new Exception("The update operation was unable to identify the properties of the type entered.");
            }

            foreach (PropertyInfo prop in typeof(T).GetRuntimeProperties())
            {
                if ("Id".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("OwnerId".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("PublisherId".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("Insertion".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                if ("Change".Equals(prop.Name.ToLower()))
                {
                    continue;
                }

                Update<T>(prop, source, target);
            }

            return target;
        }

        private static void ToAssign<S, T>(PropertyInfo prop, S source, T target)
        {
            if (prop.GetMethod == null || prop.GetMethod == default)
            {
                return;
            }

            if (prop.SetMethod == null || prop.SetMethod == default)
            {
                return;
            }

            typeof(S).GetProperties().Any(p =>
            {
                if (p.Name != prop.Name || p.PropertyType != prop.PropertyType)
                {
                    return false;
                }

                prop.SetValue(target, p.GetValue(source));

                return true;
            });
        }

        private static void Update<T>(PropertyInfo prop, T source, T target)
        {
            if (prop.GetMethod == null || prop.GetMethod == default)
            {
                return;
            }

            if (prop.SetMethod == null || prop.SetMethod == default)
            {
                return;
            }

            prop.SetValue(target, prop.GetValue(source));
        }

        public static T SoftCopy<T>(T source)
        {
            var result = Activator.CreateInstance<T>();

            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.GetMethod == null || prop.GetMethod == default)
                {
                    continue;
                }

                if (prop.SetMethod == null || prop.SetMethod == default)
                {
                    continue;
                }

                prop.SetValue(result, prop.GetValue(source));
            }

            return result;
        }

        public static object SoftCopy(object source, Type type)
        {
            var result = Activator.CreateInstance(type);

            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.GetMethod == null || prop.GetMethod == default)
                {
                    continue;
                }

                if (prop.SetMethod == null || prop.SetMethod == default)
                {
                    continue;
                }

                prop.SetValue(result, prop.GetValue(source));
            }

            return result;
        }

        public static T CreateWithValues<T>(DbPropertyValues values) where T : new()
        {
            T entity = new T();
            Type type = typeof(T);

            foreach (var name in values.PropertyNames)
            {
                var property = type.GetProperty(name);
                property.SetValue(entity, values.GetValue<object>(name));
            }

            return entity;
        }

        public static object CreateWithValues(PropertyValues values, Type type)
        {
            var entity = Activator.CreateInstance(type);

            if (typeof(IBaseStateful).IsAssignableFrom(type))
            {
                ((IBaseStateful)entity).State = true;
            }

            foreach (var prop in values.Properties)
            {
                CreateWithValues(values, entity, type, prop);
            }

            return entity;
        }

        private static void CreateWithValues(PropertyValues values, object entity, Type type, IProperty prop)
        {
            try
            {
                type.GetProperty(prop.Name)?.SetValue(entity, values.GetValue<object>(prop));

                return;
            }
            catch (Exception)
            {

            }

            try
            {
                type.GetProperty(prop.Name)?.SetValue(entity, values.GetValue<Guid?>(prop));

                return;
            }
            catch (Exception)
            {

            }

            try
            {
                type.GetProperty(prop.Name)?.SetValue(entity, values.GetValue<DateTime>(prop));

                return;
            }
            catch (Exception)
            {

            }

            try
            {
                type.GetProperty(prop.Name)?.SetValue(entity, values.GetValue<bool>(prop));

                return;
            }
            catch (Exception)
            {

            }
        }
    }
}