using System;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using genX;
using genX.Encoding;
using genX.Mutation;
//using genX.Recombination;
using genX.Scaling;
using genX.Selection;
using genX.Termination;

namespace genX.Encoding
{
    internal class TypeImporter
    {
        System.Type type;
        public TypeImporter(System.Type type)
        {
            this.type = type;
        }

        public GeneDescriptor[] ImportType()
        {
            GeneDescriptor[] geneDescriptors;
            FieldInfo[] fields;
            fields = type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            geneDescriptors = new GeneDescriptor[fields.Length];
            int i=0;
            foreach(FieldInfo field in fields)
            {
                System.Type fieldType = field.FieldType;
                switch(fieldType.ToString())
                {
                    case "System.Int32":
                        geneDescriptors[i] = new IntegerGeneDescriptor(0, 1000);                        
                        geneDescriptors[i].Name = field.Name;
                        break;
                    case "System.Double":
                        geneDescriptors[i] = new DoubleGeneDescriptor();
                        geneDescriptors[i].Name = field.Name;
                        break;
                }   
                System.Diagnostics.Debug.WriteLine(field.ToString() + ": " + field.FieldType.ToString());
                i++;
            }
            return geneDescriptors;
        }

        public object ChromosomeToObject(Chromosome c)
        {
            object o = Activator.CreateInstance(type);
            foreach(Gene g in c.Genes)
            {
                GeneDescriptor desc = g.Descriptor;
                FieldInfo fi = type.GetField(desc.Name);
                fi.SetValue(o,g.ObjectValue);
            }
            return o;
        }
    }
}