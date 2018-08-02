using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using System.Configuration;
namespace MySchool.DAL
{
  public static  class SQLHelper
    {
      //用静态的方法调用的时候不用创建SQLHelper的实例
      //Execetenonquery
     // public static string Constr = "server=HAPPYPIG\\SQLMODEL;database=shooltest;uid=sa;pwd=6375196;";
      public static string Constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
      
      /// <summary>
      /// 执行NonQuery命令
      /// </summary>
      /// <param name="cmdTxt"></param>
      /// <param name="parames"></param>
      /// <returns></returns>
      public static int ExecuteNonQuery(string cmdTxt, params SqlParameter[] parames)
      {
          return ExecuteNonQuery(cmdTxt, CommandType.Text, parames);
      }
      //可以使用存储过程的ExecuteNonquery
      public static int ExecuteNonQuery(string cmdTxt, CommandType cmdtype, params SqlParameter[] parames)
      {
          //判断脚本是否为空 ，直接返回0
          if (string.IsNullOrEmpty(cmdTxt))
          {
              return 0;
          }
          using (SqlConnection con = new SqlConnection(Constr))
          {
              using (SqlCommand cmd = new SqlCommand(cmdTxt, con))
              {
                  if (parames != null)
                  {
                      cmd.CommandType = cmdtype;
                      cmd.Parameters.AddRange(parames);
                  }
                  con.Open();
                  return cmd.ExecuteNonQuery();
              }
          }
      }
      public static SqlDataReader ExecuteDataReader(string cmdTxt, params SqlParameter[] parames)
      {
          return ExecuteDataReader(cmdTxt, CommandType.Text, parames);
      }
      //SQLDataReader存储过程方法
      public static SqlDataReader ExecuteDataReader(string cmdTxt, CommandType cmdtype, params SqlParameter[] parames)
      {
          if (string.IsNullOrEmpty(cmdTxt))
          {
              return null;
          }
          SqlConnection con = new SqlConnection(Constr);

          using (SqlCommand cmd = new SqlCommand(cmdTxt, con))
          {
              cmd.CommandType = cmdtype;
              if (parames != null)
              {
                  
                  cmd.Parameters.AddRange(parames);
              }
              con.Open();
              //把reader的行为加进来。当reader释放资源的时候，con也被一块关闭
              return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
          }

      }
      public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parames)
      {
          return ExecuteDataTable(sql, CommandType.Text, parames);
      }
      //调用存储过程的类，关于(ExecuteDataTable)
      public static DataTable ExecuteDataTable(string sql, CommandType cmdType, params SqlParameter[] parames)
      {
          if (string.IsNullOrEmpty(sql))
          {
              return null;
          }
          DataTable dt = new DataTable();
          using (SqlDataAdapter da = new SqlDataAdapter(sql, Constr))
          {
              da.SelectCommand.CommandType = cmdType;
              if (parames != null)
              {
                  da.SelectCommand.Parameters.AddRange(parames);
              }
              da.Fill(dt);
              return dt;
          }
      }
    
      /// <summary>
      /// ExecuteScalar
      /// </summary>
      /// <param name="cmdTxt">第一个参数，SQLServer语句</param>
      /// <param name="parames">第二个参数，传递0个或者多个参数</param>
      /// <returns></returns>
      public static object ExecuteScalar(string cmdTxt, params SqlParameter[] parames)
      {
          return ExecuteScalar(cmdTxt, CommandType.Text, parames);
      }
      //可使用存储过程的ExecuteScalar
      public static object ExecuteScalar(string cmdTxt, CommandType cmdtype, params SqlParameter[] parames)
      {
          if (string.IsNullOrEmpty(cmdTxt))
          {
              return null;
          }
          using (SqlConnection con = new SqlConnection(Constr))
          {
              using (SqlCommand cmd = new SqlCommand(cmdTxt, con))
              {
                  cmd.CommandType = cmdtype;
                  if (parames != null)
                  {
                      cmd.Parameters.AddRange(parames);
                  }
                  con.Open();
                return   cmd.ExecuteScalar();
              }
          }
          
      }
      //调用存储过程的DBHelper类（关于ExeceutScalar,包含事务,只能处理Int类型，返回错误号）
      public static object ExecuteScalar(string cmdTxt, CommandType cmdtype,SqlTransaction sqltran, params SqlParameter[] parames)
      {
           if (string.IsNullOrEmpty(cmdTxt))
          {
              return 0;
          }
          using (SqlConnection con = new SqlConnection(Constr))
          {
              int sum = 0;
              using (SqlCommand cmd = new SqlCommand(cmdTxt, con))
              {
                  cmd.CommandType=cmdtype;
                  if (parames != null)
                  {
                      cmd.Parameters.AddRange(parames);
                  }
                  con.Open();
                  sqltran = con.BeginTransaction();
                  try
                  {
                      cmd.Transaction = sqltran;
                      sum=Convert.ToInt32( cmd.ExecuteScalar());
                      sqltran.Commit();
                  }
                  catch (SqlException ex)
                  {
                      sqltran.Rollback();
                  }
                  return sum;
              }
          }
      }
    }
}

