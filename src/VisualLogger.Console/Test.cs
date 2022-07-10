using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Reader;
using VisualLogger.Sources;

namespace VisualLogger.Console
{
    internal class Test
    {
        volatile int index = 0;

        public Test()
        {
            //string p = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3})";
            string p = @"^(\d{2}\/\d{2}\/\d{2} \d{2}:\d{2}:\d{2}.\d{3})";
            string pc = @"^(\d{2}\/\d{2}\/\d{2} \d{2}:\d{2}:\d{2}.\d{3}) \<(.*?)\> \[(.*?)\] (.*?) (.*)";
            //var lines = File.ReadLines("xxxxx.txt");
            //var lines = File.ReadLines("C:\\Users\\Jim.Jiang\\Downloads\\RZHO0S4V-C\\private\\var\\mobile\\Containers\\Data\\Application\\626168FA-07C1-4DC3-B0E3-072222D402CD\\Documents\\log\\2022-07-01-064730.643-action.log");
            //var lines = File.ReadLines("C:\\Users\\Jim.Jiang\\Downloads\\WRoomsFeedBack_HostLog_84044ae1-6221-448c-bb1a-dc26bc11b2ae_20220701-041517\\RoomsHost-20220629_192324-pid_2800.1.log");
            ReadLinesIterator lines = new ReadLinesIterator(new StreamReader(File.OpenRead("C:\\Users\\Jim.Jiang\\Downloads\\WRoomsFeedBack_HostLog_84044ae1-6221-448c-bb1a-dc26bc11b2ae_20220701-041517\\RoomsHost-20220629_192324-pid_2800.1.log")));
            //var lines = Enumerable.Range(0, 99999).Select(x => x.ToString());

            //var count = lines.Count();

            //lines.MoveNext();
            //lines.MoveNext();
            //lines.MoveNext();
            //lines.MoveNext();
            //lines.MoveNext();
            //lines.MoveNext();
            //lines.MoveNext();
            //lines.MoveNext();
            //lines.MoveNext();
            //349428
            int index = 0;
            var s = lines
             .AsParallel()
             .AsOrdered()
             .Select(l =>
             {
                 return (Regex.IsMatch(l, p), l);
             })
             .AsSequential()
             .Select(x =>
             {
                 if (x.Item1)
                 {
                     index++;
                 }
                 return (index, x.l);
             })
             .ToLookup(x => x.index, x => x.l)
             .AsParallel()
             .AsOrdered()
             .Select(x =>
             {
                 var content = string.Join("\r\n", x);
                 var match = Regex.Match(content, pc, RegexOptions.Singleline);
                 var captureCells = match.Groups.Values.Skip(1).ToArray();
                 return captureCells;
             })
             .ToArray();

        }


    }
    internal class Test1
    {
        public Test1()
        {

            var expression = GetExpression<Test2>();
            expression.Invoke("xxxxxxx");
        }
        public static Func<string, T> GetExpression<T>()
        {
            var argumentType = new[] { typeof(string) };
            // Get the Constructor which matches the given argument Types:
            var constructor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.HasThis,
                argumentType,
                new ParameterModifier[0]);

            // Get a set of Expressions representing the parameters which will be passed to the Func:
            var lamdaParameterExpressions = GetLambdaParameterExpressions(argumentType).ToArray();

            // Get a set of Expressions representing the parameters which will be passed to the constructor:
            var constructorParameterExpressions = GetConstructorParameterExpressions(
                lamdaParameterExpressions,
                argumentType).ToArray();

            // Get an Expression representing the constructor call, passing in the constructor parameters:
            var constructorCallExpression = Expression.New(constructor, constructorParameterExpressions);

            // Compile the Expression into a Func which takes three arguments and returns the constructed object:
            var constructorCallingLambda = Expression
                .Lambda<Func<string, T>>(constructorCallExpression, lamdaParameterExpressions)
                .Compile();
            return constructorCallingLambda;

        }

        private static IEnumerable<ParameterExpression> GetLambdaParameterExpressions(Type[] argumentTypes)
        {
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                yield return Expression.Parameter(typeof(object), string.Concat("param", i));
            }
        }

        private static IEnumerable<UnaryExpression> GetConstructorParameterExpressions(
    ParameterExpression[] lamdaParameterExpressions,
    Type[] constructorArgumentTypes)
        {
            for (int i = 0; i < constructorArgumentTypes.Length; i++)
            {
                // Each parameter passed to the lambda is of type object, so we need to convert it into 
                // the appropriate type for the constructor:
                yield return Expression.Convert(lamdaParameterExpressions[i], constructorArgumentTypes[i]);
            }
        }
    }
    internal class Test2
    {
        private Test2(string A)
        {

        }

        public string P1 { get; set; }
    }
}
