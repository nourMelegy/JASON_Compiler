using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }

        //Program → FunctionList Main_Function
        Node Program()
        {
            Node program = new Node("Program");

            program.Children.Add(FunctionList());
            program.Children.Add(Main_Function());

            return program;
        }

        //FunctionList → Function_Statement FunctionList | ε
        Node FunctionList()
        {
            Node functionList = new Node("FunctionList");

            while (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                functionList.Children.Add(Function_Statement());
            }

            return functionList;
        }

        //Main_Function → Datatype main ( ) Function_Body
        Node Main_Function()
        {
            Node mainFunc = new Node("Main_Function");

            mainFunc.Children.Add(Datatype());
            mainFunc.Children.Add(match(Token_Class.Main));
            mainFunc.Children.Add(match(Token_Class.LParanthesis));
            mainFunc.Children.Add(match(Token_Class.RParanthesis));
            mainFunc.Children.Add(Function_Body());

            return mainFunc;
        }

        //Function_Declaration → Datatype FunctionName ( ParameterList )
        Node Function_Declaration()
        {
            Node decl = new Node("Function_Declaration");

            decl.Children.Add(Datatype());
            decl.Children.Add(match(Token_Class.Identifier));
            decl.Children.Add(match(Token_Class.LParanthesis));
            decl.Children.Add(ParameterList());
            decl.Children.Add(match(Token_Class.RParanthesis));

            return decl;
        }

        //ParameterList → Parameter MoreParameters | ε
        Node ParameterList()
        {
            Node paramList = new Node("ParameterList");

            if (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
            {
                paramList.Children.Add(Parameter());
                paramList.Children.Add(MoreParameters());
            }

            return paramList;
        }

        //MoreParameters → , Parameter MoreParameters | ε
        Node MoreParameters()
        {
            Node moreParams = new Node("MoreParameters");

            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                moreParams.Children.Add(match(Token_Class.Comma));
                moreParams.Children.Add(Parameter());
                moreParams.Children.Add(MoreParameters());
            }

            return moreParams;
        }

        //Parameter → Datatype Identifier
        Node Parameter()
        {
            Node param = new Node("Parameter");

            param.Children.Add(Datatype());
            param.Children.Add(match(Token_Class.Identifier));

            return param;
        }

        //Datatype → int | float | string
        Node Datatype()
        {
            Node datatype = new Node("Datatype");

            if (TokenStream[InputPointer].token_type == Token_Class.Int)
                datatype.Children.Add(match(Token_Class.Int));
            else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                datatype.Children.Add(match(Token_Class.Float));
            else if (TokenStream[InputPointer].token_type == Token_Class.String)
                datatype.Children.Add(match(Token_Class.String));
            else
            {
                Errors.Error_List.Add("Expected Datatype");
                InputPointer++;
            }

            return datatype;
        }


        // Implement your logic here
        Node Condition_Statement()
        {
            Node condition_statement = new Node("Condition_Statement");
            condition_statement.Children.Add(Condition());
            condition_statement.Children.Add(Condition_Ext());
            return condition_statement;
        }
        Node Condition_Ext()
        {
            Node condition_Ext = new Node("Condition_Ext");
            if (InputPointer < TokenStream.Count &&
       (TokenStream[InputPointer].token_type == Token_Class.And ||
        TokenStream[InputPointer].token_type == Token_Class.Or))
            {
                condition_Ext.Children.Add(Boolean_Operator());
                condition_Ext.Children.Add(Condition());
                condition_Ext.Children.Add(Condition_Ext());
            }
            return condition_Ext;
        }
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }
        Node Condition_Operator()
        {
            Node condition_operator = new Node("Condition_Operator");
            if (InputPointer < TokenStream.Count)
            {
                Token_Class currentTokenType = TokenStream[InputPointer].token_type;

                if (currentTokenType == Token_Class.Equal)
                    condition_operator.Children.Add(match(Token_Class.Equal));
                else if (currentTokenType == Token_Class.NotEqual)
                    condition_operator.Children.Add(match(Token_Class.NotEqual));
                else if (currentTokenType == Token_Class.GreaterT)
                    condition_operator.Children.Add(match(Token_Class.GreaterT));
                else if (currentTokenType == Token_Class.LessT)
                    condition_operator.Children.Add(match(Token_Class.LessT));
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Condition Operator and " +
                        TokenStream[InputPointer].token_type.ToString() + " found\r\n");
                    InputPointer++;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Unexpected end of input, Expected Condition Operator\r\n");
            }
            return condition_operator;
        }
        Node Boolean_Operator()
        {
            Node boolean_operator = new Node("Boolean_Operator");
            if (InputPointer < TokenStream.Count)
            {
                Token_Class currentTokenType = TokenStream[InputPointer].token_type;

                if (currentTokenType == Token_Class.And)
                    boolean_operator.Children.Add(match(Token_Class.And));
                else if (currentTokenType == Token_Class.Or)
                    boolean_operator.Children.Add(match(Token_Class.Or));
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Boolean Operator and " +
                        TokenStream[InputPointer].token_type.ToString() + " found\r\n");
                    InputPointer++;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Unexpected end of input, Expected Boolean Operator\r\n");
            }
            return boolean_operator;
        }
        Node Function_Call()
        {
            Node function_call = new Node("Function_Call");
            function_call.Children.Add(match(Token_Class.Identifier));
            function_call.Children.Add(match(Token_Class.LParanthesis));
            if (InputPointer < TokenStream.Count &&
                TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                function_call.Children.Add(Arguments());
            }
            function_call.Children.Add(match(Token_Class.RParanthesis));
            function_call.Children.Add(match(Token_Class.Semicolon));
            return function_call;
        }
        Node Arguments()
        {
            Node arguments = new Node("Arguments");
            arguments.Children.Add(match(Token_Class.Identifier));
            arguments.Children.Add(Arguments_Tail());
            return arguments;
        }
        Node Arguments_Tail()
        {
            Node arguments_tail = new Node("Arguments_Tail");
            if (InputPointer < TokenStream.Count &&
                TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                arguments_tail.Children.Add(match(Token_Class.Comma));
                arguments_tail.Children.Add(match(Token_Class.Identifier));
                arguments_tail.Children.Add(Arguments_Tail());
            }
            return arguments_tail;
        }
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }

        /*
        Node Header()
        {
            Node header = new Node("Header");
            // write your code here to check the header sructure
            return header;
        }
        Node DeclSec()
        {
            Node declsec = new Node("DeclSec");
            // write your code here to check atleast the declare sturcure 
            // without adding procedures
            return declsec;
        }
        Node Block()
        {
            Node block = new Node("block");
            // write your code here to match statements
            return block;
        }
        */
    }
}
