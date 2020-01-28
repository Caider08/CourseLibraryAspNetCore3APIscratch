using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CourseLibrary.API.Helpers
{
    public static class ObjectExtensions
    {

        public static ExpandoObject ShapeData<TSource>(
           this TSource source,
           string fields)

        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dataShapedObject = new ExpandoObject();


            if (string.IsNullOrWhiteSpace(fields))
            {
                //all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

               // foreach(var propertyInfo in propertyInfos)
                //{
                    //get the value of the property on the source object


                //}



            }
            else
            {
                //the field are separated by "," so we split it.
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    //trim each field, as it might contain leading
                    //or trailing spaces.  Can't trim the var in foreach
                    //so use another var.
                    var propertyName = field.Trim();

                    //use Reflection to get the property on the source object
                    //we need to include public and instance, b/c specifying a binding
                    //flag overrites the already-existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on " + $" {typeof(TSource)}");
                    }

                    //add propertyInfo to list
                    propertyInfoList.Add(propertyInfo);
                }
            }

            //run through the source objects
            foreach (TSource sourceObject in source)
            {
                //create an ExpandoObject that will hold the 
                //selected properties & Values
                var dataShapedObject = new ExpandoObject();

                //Get the value of each property we have to return.  For that
                //we run through the list
                foreach (var propertyInfo in propertyInfoList)
                {
                    //GetValue returns the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    //add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                //add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            //return the list
            return expandoObjectList;


        }
    }
}
}
