using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json;
using System.IO;

namespace Lab1
{
    public class Input
    {
        public int K { get; set; }
        public decimal[] Sums { get; set; }
        public int[] Muls { get; set; }
    }

    public class Output
    {
        public decimal SumResult { get; set; }
        public int MulResult { get; set; }
        public decimal[] SortedInputs { get; set; }
    }

    public static class XmlTools
    {
        public static T FromXmlString<T>(this T input, string str)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (TextReader reader = new StringReader(str))
            {
                input = (T)serializer.Deserialize(reader);
            }

            return input;
        }

        public static string ToXmlString<T>(this T input)
        {
            using (var writer = new StringWriter())
            {
                input.ToXml(writer);
                return writer.ToString();
            }
        }
        public static void ToXml<T>(this T objectToSerialize, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, objectToSerialize);
        }

        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
        }
    }


    class Program
    {
        public static readonly List<string> jsonTypeChecks = new List<string> {"JSON", "Json", "json"};
        public static readonly List<string> xmlTypeChecks = new List<string> { "XML", "Xml", "xml" };

        static int Main(string[] args)
        {
            Input readedInfo;
            Output outInfo;

            //StreamReader reader = new StreamReader(pathToFile);

            bool isXML = true;

            string type = null;
            string obj = null;

            type = Console.ReadLine();

            if (jsonTypeChecks.Contains(type)) isXML = false;
            else if(!xmlTypeChecks.Contains(type))
            {
                Console.WriteLine("Wrong type. Only JSON or XML are allowed.");
                return -1;
            }

            obj = Console.ReadLine();

            readedInfo = DeserializeInput(obj, isXML);

            if (readedInfo == null) return -2;

            outInfo = CreateOutput(readedInfo);

            Output(outInfo, isXML);

            return 0;
        }

        static private Input DeserializeInput(string rawInput, bool isXML)
        {
            Input input = null;

            if (isXML)
            {
                try
                {
                    input = input.FromXmlString(rawInput);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Invalid operation detected. Most likely that text is not in XML format. Please try to use XML format or check text for mistakes.");
                    //Console.WriteLine(ex);
                }
            }
            else
            {
                try
                {
                    input = JsonSerializer.Deserialize<Input>(rawInput);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Invalid operation detected. Most likely that text is not in JSON format. Please try to use JSON format or check text for mistakes.");
                    //Console.WriteLine(ex);
                }
            }

            return input;
        }

        static private Output CreateOutput(Input input)
        {
            Output output = new Output();

            decimal sum = 0;
            int mul = 1;
            List<decimal> sortedOrign = new List<decimal>();

            foreach(var number in input.Sums)
            {
                sum += number;
                sortedOrign.Add(number);
            }

            sum *= input.K;

            foreach (var number in input.Muls)
            {
                mul *= number;
                sortedOrign.Add(number);
            }

            sortedOrign.Sort();

            output.SumResult = sum;
            output.MulResult = mul;
            output.SortedInputs = sortedOrign.ToArray();

            return output;
        }

        static private void Output(Output output, bool isXml)
        {
            string outputStr = null;

            if(isXml)
            {
                outputStr = output.ToXmlString();
            }
            else
            {
                outputStr = JsonSerializer.Serialize(output);
            }

            Console.WriteLine(outputStr);
        }
    }
}
