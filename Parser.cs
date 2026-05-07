using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

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
        public  Node root;
       
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
        
        // Expression -> String | Equation
Node Expression()
{
    Node expression = new Node("Expression");

    if (InputPointer<TokenStream.Count &&
        TokenStream[InputPointer].token_type == Token_Class.StringLiteral)
    {
        expression.Children.Add(match(Token_Class.StringLiteral));
    }
     else if (InputPointer < TokenStream.Count &&
                    TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                expression.Children.Add(match(Token_Class.Endl)); //not in cfg but had to be handled, can be deleted if handled somewhere else
            }
            else
    {
        expression.Children.Add(Equation());
    }

return expression;
}
         // Equation -> Factor Equation'
 Node Equation()
 {
     Node equation = new Node("Equation");
     equation.Children.Add(Factor());
     equation.Children.Add(Equation_tail());
     return equation;
 }
 Node Equation_tail()
 {
     Node equation_prime = new Node("Equation_tail");

     if (InputPointer < TokenStream.Count &&
         (TokenStream[InputPointer].token_type == Token_Class.Plus ||
          TokenStream[InputPointer].token_type == Token_Class.Minus))
     {
         equation_prime.Children.Add(AddOp());
         equation_prime.Children.Add(Factor());
         equation_prime.Children.Add(Equation_tail());
     }

     return equation_prime;
 }
          Node Factor()
  {
      Node factor = new Node("Factor");
      factor.Children.Add(Term());
      factor.Children.Add(Factor_tail());
      return factor;
  }
  Node Factor_tail()
  {
      Node factor_prime = new Node("Factor_tail");

      if (InputPointer < TokenStream.Count &&
          (TokenStream[InputPointer].token_type == Token_Class.Multiply ||
           TokenStream[InputPointer].token_type == Token_Class.Divide))
      {
          factor_prime.Children.Add(MultOp());
          factor_prime.Children.Add(Term());
          factor_prime.Children.Add(Factor_tail());
      }

      return factor_prime;
  }
         // MultOp -> * | /
  Node MultOp()
 {
     Node multOp = new Node("MultOp");

     if (InputPointer < TokenStream.Count)
     {
         if (TokenStream[InputPointer].token_type == Token_Class.Multiply)
             multOp.Children.Add(match(Token_Class.Multiply));
         else if (TokenStream[InputPointer].token_type == Token_Class.Divide)
             multOp.Children.Add(match(Token_Class.Divide));
         else
         {
             Errors.Error_List.Add("Parsing Error: Expected '*' or '/' but found " +
                 TokenStream[InputPointer].token_type.ToString() + "\r\n");
             InputPointer++;
         }
     }
     else
     {
         Errors.Error_List.Add("Parsing Error: Unexpected end of input, Expected MultOp\r\n");
     }

     return multOp;
 }
 // AddOp -> + | -
 Node AddOp()
 {
     Node addOp = new Node("AddOp");

     if (InputPointer < TokenStream.Count)
     {
         if (TokenStream[InputPointer].token_type == Token_Class.Plus)
             addOp.Children.Add(match(Token_Class.Plus));
         else if (TokenStream[InputPointer].token_type == Token_Class.Minus)
             addOp.Children.Add(match(Token_Class.Minus));
         else
         {
             Errors.Error_List.Add("Parsing Error: Expected '+' or '-' but found " +
                 TokenStream[InputPointer].token_type.ToString() + "\r\n");
             InputPointer++;
         }
     }
     else
     {
         Errors.Error_List.Add("Parsing Error: Unexpected end of input, Expected AddOp\r\n");
     }

     return addOp;
 }
     // Term -> Number | Identifier | Function_Call | ( Equation )
 Node Term()
 {
     Node term = new Node("Term");

     if (InputPointer < TokenStream.Count)
     {
         if (TokenStream[InputPointer].token_type == Token_Class.Constant)
         {
             term.Children.Add(match(Token_Class.Constant));
         }
         else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
         {
             if (InputPointer + 1 < TokenStream.Count &&
                 TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
             {
                 term.Children.Add(Function_Call()); // option 2 - clean single call
             }
             else
             {
                 term.Children.Add(match(Token_Class.Identifier));
             }
         }
         else if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
         {
             term.Children.Add(match(Token_Class.LParanthesis));
             term.Children.Add(Equation());
             term.Children.Add(match(Token_Class.RParanthesis));
         }
         else
         {
             Errors.Error_List.Add("Parsing Error: Expected Term but found " +
                 TokenStream[InputPointer].token_type.ToString() + "\r\n");
             InputPointer++;
         }
     }
     else
     {
         Errors.Error_List.Add("Parsing Error: Unexpected end of input, Expected Term\r\n");
     }

     return term;
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
        Node Function_Body()
        {
            Node FnBody = new Node("Function_Body");

            //Function_Body → { Statements Return_Statement } | { Statements }
            //
            FnBody.Children.Add(match(Token_Class.LCurly));
            FnBody.Children.Add(Statements());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                FnBody.Children.Add(match(Token_Class.Return));
                FnBody.Children.Add(Expression());
            }
            FnBody.Children.Add(match(Token_Class.RCurly));

            return FnBody;
        }
        Node Function_Statement()
        {
            //Function_Statement → Function_Declaration Function_Body
            Node FnStatement = new Node("Function_Statement");
            FnStatement.Children.Add(Function_Declaration());
            FnStatement.Children.Add(Function_Body());
            return FnStatement;
        }
        Node FnBody = new Node("Function_Body");
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




        Node Statements()
        {
            Node Statements = new Node("Statements");
            Statements.Children.Add(Statement());
            Statements.Children.Add(StatementsDash());
            return Statements;

        }

        Node Statement()
        {
            Node Statement = new Node("Statement");
            if (InputPointer < TokenStream.Count)
            {
                //read statement
                if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    Statement.Children.Add(match(Token_Class.Read));
                    Statement.Children.Add(match(Token_Class.Identifier));
                }
                //write statement
                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    Statement.Children.Add(match(Token_Class.Write));
                    Statement.Children.Add(match(Token_Class.Identifier));
                }
                //return statement
                else if (TokenStream[InputPointer].token_type == Token_Class.Return)
                {
                    Statement.Children.Add(match(Token_Class.Return));
                    Statement.Children.Add(Expression());
                }
                // Id_Tail statement
                else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    Statement.Children.Add(match(Token_Class.Identifier));
                    
                    Statement.Children.Add(Statement_Id_Tail());
                }
                //repeat statement
                else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    Statement.Children.Add(match(Token_Class.Repeat));
                    Statement.Children.Add(Statements());
                    Statement.Children.Add(match(Token_Class.Until));
                    Statement.Children.Add(Condition_Statement());
                }
                //if statement
                else if (TokenStream[InputPointer].token_type == Token_Class.If)
                {
                    Statement.Children.Add(match(Token_Class.If));
                    Statement.Children.Add(Condition_Statement());
                    Statement.Children.Add(match(Token_Class.Then));
                    Statement.Children.Add(Statements());
                    Statement.Children.Add(IfTail());
                    Statement.Children.Add(match(Token_Class.End));
                }
                //declaration statement
                else if (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    if(TokenStream[InputPointer].token_type == Token_Class.Int)
                    {
                        Statement.Children.Add(match(Token_Class.Int));
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                    {
                        Statement.Children.Add(match(Token_Class.Float));
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.String)
                    {
                        Statement.Children.Add(match(Token_Class.String));
                    }
                    Statement.Children.Add(IdList());
                }
                else
                {
                    return null;
                }
            }
            return Statement;

        }
        Node StatementsDash()
        {
            Node Statements_dash = new Node("StatementsDash");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Semicolon)
                {
                    Statements_dash.Children.Add(match(Token_Class.Semicolon));
                    Statements_dash.Children.Add(Statement());
                    Statements_dash.Children.Add(StatementsDash());
                }
                else
                {
                    return null;
                }

            }
            return Statements_dash;
        }
        Node IfTail() {
            Node If_Tail = new Node("IfTail");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ElseIf)
                {
                    If_Tail.Children.Add(match(Token_Class.ElseIf));
                    If_Tail.Children.Add(Condition());
                    If_Tail.Children.Add(match(Token_Class.Then));
                    If_Tail.Children.Add(Statements());
                    If_Tail.Children.Add(IfTail());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    If_Tail.Children.Add(match(Token_Class.Else));
                    If_Tail.Children.Add(Statements());

                }
                else
                {
                    return null;

                }
            }
            return If_Tail;
        }
        Node IdList() {
            Node Id_List = new Node("IdList");
            Id_List.Children.Add(match(Token_Class.Identifier));
            Id_List.Children.Add(IdListDash());
            return Id_List;
        }
        Node IdListDash()
        {
            Node Id_List_Dash = new Node("IdListDash");
            Id_List_Dash.Children.Add(match(Token_Class.Semicolon));
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Semicolon)
                {
                    Id_List_Dash.Children.Add(match(Token_Class.Semicolon));
                    Id_List_Dash.Children.Add(match(Token_Class.Identifier));
                    Id_List_Dash.Children.Add(IdListDash());
                }
                else
                {
                    return null;
                }


            }

            return Id_List_Dash;
        }

        Node Statement_Id_Tail() {
            Node Id_Tail = new Node("Id_Tail");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Assign)
                {
                    Id_Tail.Children.Add(match(Token_Class.Assign));
                    Id_Tail.Children.Add(Expression());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    Id_Tail.Children.Add(match(Token_Class.LParanthesis));
                    if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                    {
                        Id_Tail.Children.Add(Arguments());
                    }

                    Id_Tail.Children.Add(match(Token_Class.RParanthesis));
                    Id_Tail.Children.Add(match(Token_Class.Semicolon));

                }
                else if (TokenStream[InputPointer].token_type == Token_Class.LessT ||
                         TokenStream[InputPointer].token_type == Token_Class.GreaterT ||
                         TokenStream[InputPointer].token_type == Token_Class.Equal ||
                         TokenStream[InputPointer].token_type == Token_Class.NotEqual)
                {
                    if (TokenStream[InputPointer].token_type == Token_Class.LessT)
                        Id_Tail.Children.Add(match(Token_Class.LessT));
                    else if (TokenStream[InputPointer].token_type == Token_Class.GreaterT)
                        Id_Tail.Children.Add(match(Token_Class.GreaterT));
                    else if (TokenStream[InputPointer].token_type == Token_Class.Equal)
                        Id_Tail.Children.Add(match(Token_Class.Equal));
                    else if (TokenStream[InputPointer].token_type == Token_Class.NotEqual)
                        Id_Tail.Children.Add(match(Token_Class.NotEqual));


                    Id_Tail.Children.Add(Term());
                    Id_Tail.Children.Add(Condition_Ext());
                }
                else
                {
                    // If it's not :=, not (, and not a condition operator, it's a syntax error!
                    Errors.Error_List.Add("Parsing Error: Expected assignment ':=', function call '(', or condition operator, but found "
                    + TokenStream[InputPointer].token_type.ToString() + "\r\n");
                    InputPointer++;
                    return null;
                }


            }
            return Id_Tail;
        }

        // Implement your logic here

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
                        + ExpectedToken.ToString()  + "\r\n");
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
    }
}
