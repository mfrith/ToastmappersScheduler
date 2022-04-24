using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Toastmappers
{
  [Serializable]
  public class MeetingModelBase
  {
    public int MeetingType { get; set; }
    public int ID { get; set; }
    public string DayOfMeeting { get; set; }
    public string Toastmaster { get; set; }
    private ObservableCollection<string> _attendees = new ObservableCollection<string>();
    public ObservableCollection<string> Attendees { get { return _attendees; } set { _attendees = value; } }
    public string Timer { get; set; }
    public bool Resolved { get; set; }

    public string WOTD { get; set; }

    public string Theme { get; set; } 
    public string Speaker1 { get; set; }
    public string Speaker2 { get; set; }
    public string GeneralEvaluator { get; set; }
    public string Evaluator1 { get; set; }
    public string Evaluator2 { get; set; }

    public string AhCounter { get; set; }
    public string Grammarian { get; set; }
    public string QuizMaster { get; set; }
    public string Video { get; set; }
    public string HotSeat { get; set; }
    private ObservableCollection<string> _guests = new ObservableCollection<string>();
    public ObservableCollection<string> Guests { get { return _guests; } set { _guests = value; } }

    public MeetingModelBase()
    {

    }
    public MeetingModelBase(string meetingTemplate, DateTime meetingDate)
    {

    }


    public MeetingModelBase Deserialize(string json)
    {
      var options = new JsonSerializerOptions
      {
        AllowTrailingCommas = true
      };

      byte[] data = Encoding.UTF8.GetBytes(json);
      Utf8JsonReader reader = new Utf8JsonReader(data, isFinalBlock: true, state: default);
      string propertyName = string.Empty;
      MeetingModelBase f = null;
      //MeetingModelRegular steve = new MeetingModelRegular();
      reader.Read();
      reader.Read();
      //var a = reader.GetString();
      //reader.Read();
      //var name = reader.GetString();

      reader.Read();
      var meetingtype = reader.GetInt32();

      if (meetingtype == 1)
        f = new MeetingModelRegular();
      else if (meetingtype == 2)
        f = new MeetingModel3Speaker();
      else if (meetingtype == 4)
        f = new MeetingModel4Speaker();
      else if (meetingtype == 5)
        f = new MeetingModel5Speaker();
      //else if (meetingtype == "10" || meetingtype == "20" || meetingtype == "30")
      //  f = new MeetingContest();
      else
        return null;

      f.MeetingType = meetingtype;

      while (reader.Read())
      {

        switch (reader.TokenType)
        {
          case JsonTokenType.PropertyName:
            {
              propertyName = reader.GetString();
              break;
            }
          case JsonTokenType.Number:
            {
              int value = reader.GetInt32();
              if (propertyName == "ID")
              {
                f.ID = value;
                break;
              }
              break;
            }
          case JsonTokenType.String:
            {
              string value = reader.GetString();

              if (propertyName == "DayOfMeeting")
                f.DayOfMeeting = value;

              if (propertyName == "Toastmaster")
                f.Toastmaster = value;

              if (propertyName == "Speaker1")
                f.Speaker1 = value;

              if (propertyName == "Speaker2")
                f.Speaker2 = value;

              if (propertyName == "Speaker3")
                (f as MeetingModel3Speaker).Speaker3 = value;

              if (propertyName == "Speaker4")
                (f as MeetingModel4Speaker).Speaker4 = value;

              if (propertyName == "GeneralEvaluator")
                f.GeneralEvaluator = value;

              if (propertyName == "Evaluator1")
                f.Evaluator1 = value;

              if (propertyName == "Evaluator2")
                f.Evaluator2 = value;

              if (propertyName == "Evaluator3")
                (f as MeetingModel3Speaker).Evaluator3 = value;

              if (propertyName == "Evaluator4")
                (f as MeetingModel4Speaker).Evaluator4 = value;

              if (propertyName == "Evaluator5")
                (f as MeetingModel5Speaker).Evaluator5 = value;

              if (propertyName == "TableTopics")
                (f as MeetingModelRegular).TableTopics = value;

              if (propertyName == "AhCounter")
                f.AhCounter = value;

              if (propertyName == "Grammarian")
                f.Grammarian = value;

              if (propertyName == "Timer")
                f.Timer = value;

              if (propertyName == "QuizMaster")
                f.QuizMaster = value;

              if (propertyName == "Video")
                f.Video = value;

              if (propertyName == "HotSeat")
                f.HotSeat = value;

              if (propertyName == "WOTD")
                f.WOTD = value;

              if (propertyName == "Theme")
                f.Theme = value;

              if (propertyName == "TTWinner")
                (f as MeetingModelRegular).TTWinner = value;

              break;

            }
          case JsonTokenType.True:
          case JsonTokenType.False:
            {
              if (propertyName == "Resolved")
              {
                bool value = reader.GetBoolean();
                f.GetType().GetProperty(propertyName).SetValue(f, value, null);
                break;
              }
              break;
            }
          case JsonTokenType.StartArray:
            {
              if (propertyName == "TTContestants")
              {
                reader.Read();
                (f as MeetingModelRegular).TTContestants = new ObservableCollection<string>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  (f as MeetingModelRegular).TTContestants.Add(reader.GetString());
                  reader.Read();
                }

                break;
              }
              if (propertyName == "Attendees")
              {
                reader.Read();
                f.Attendees = new ObservableCollection<string>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  f.Attendees.Add(reader.GetString());
                  reader.Read();
                }

                break;
              }

              if (propertyName == "Guests")
              {
                reader.Read();
                f.Guests = new ObservableCollection<string>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  f.Guests.Add(reader.GetString());
                  reader.Read();
                }

                break;
              }
              break;
            }

        }

      }
      return f;
      //return JsonSerializer.Parse<MeetingModelRegular>(json, options);

    }
    public string Serialize(MeetingModelBase meeting)
    {

      int theMeeting = meeting.MeetingType;
      using (var stream = new System.IO.MemoryStream())
      {
        using (var writer = new System.Text.Json.Utf8JsonWriter(stream))
        {
          writer.WriteStartObject();
          writer.WriteNumber("MeetingType", meeting.MeetingType);
          writer.WriteNumber("ID", meeting.ID);
          writer.WriteString("DayOfMeeting", meeting.DayOfMeeting);
          writer.WriteString("Toastmaster", meeting.Toastmaster);
          if (theMeeting == 1)
            (meeting as MeetingModelRegular).Serialize(writer);

          //if (meeting.Attendees != null)
          //{
            writer.WriteStartArray("Attendees");
            foreach (string m in meeting.Attendees)
            {
              writer.WriteStringValue(m);
            }
            writer.WriteEndArray();
          //}
          writer.WriteBoolean("Resolved", meeting.Resolved);
          writer.WriteString("WOTD", meeting.WOTD);
          writer.WriteString("Theme", meeting.Theme);
          writer.WriteStartArray("Guests");
          foreach (string m in meeting.Guests)
          {
            writer.WriteStringValue(m);
          }
          writer.WriteEndArray();
          writer.WriteEndObject();
        }
        return Encoding.UTF8.GetString(stream.ToArray());
      }
        //if (theMeeting == "1")
        //  return (meeting as MeetingModelRegular).Serialize(meeting);
        //else if (theMeeting == "2")
        //  return (meeting as MeetingModel3Speaker).Serialize(meeting);
        //else if (theMeeting == "3")
        //  return (meeting as MeetingModel3Speaker).Serialize(meeting);
      //using (var stream = new System.IO.MemoryStream())
      //  {
      //    using (var writer = new System.Text.Json.Utf8JsonWriter(stream))//, options))
      //    {
      //      writer.WriteStartObject();
      //      writer.WriteString("MeetingType", meeting.MeetingType);
      //      writer.WriteString("ID", meeting.ID);
      //      writer.WriteString("DayOfMeeting", meeting.DayOfMeeting);
      //      writer.WriteString("Toastmaster", meeting.Toastmaster);
      //      writer.WriteString("Speaker1", meeting.Speaker1);
      //      writer.WriteString("Speaker2", meeting.Speaker2);
      //      writer.WriteString("GeneralEvaluator", meeting.GeneralEvaluator);
      //      writer.WriteString("Evaluator1", meeting.Evaluator1);
      //      writer.WriteString("Evaluator2", meeting.Evaluator2);
      //      writer.WriteString("TableTopics", (meeting as MeetingModelRegular).TableTopics);
      //      writer.WriteString("AhCounter", meeting.AhCounter);
      //      writer.WriteString("Grammarian", meeting.Grammarian);
      //      writer.WriteString("Timer", meeting.Timer);
      //      writer.WriteString("QuizMaster", meeting.QuizMaster);
      //      writer.WriteString("Video", meeting.Video);
      //      writer.WriteString("HotSeat", meeting.HotSeat);
      //      writer.WriteString("Attendees", meeting.Attendees?.ToString());
      //      writer.WriteString("Resolved", meeting.Resolved);
      //      writer.WriteString("WOTD", meeting.WOTD);
      //      writer.WriteString("Theme", meeting.Theme);
      //      writer.WriteString("TTContestants", (meeting as MeetingModelRegular).TTContestants?.ToString());
      //      writer.WriteString("TTWinner", (meeting as MeetingModelRegular).TTWinner);
      //      writer.WriteEndObject();
      //    }

      //    return Encoding.UTF8.GetString(stream.ToArray());
      //  }
      //}
      //return theMeeting;
    }
  }

  public class MeetingContest : MeetingModelBase
  {
    public List<string> Contestants { get; set; }
    public string ChiefJudge { get; set; }

    public MeetingContest()
    {

    }
  }

  [Serializable]
  public class MeetingModelRegular : MeetingModelBase
  {
    //public string MeetingType { get; set; }
    //public string ID { get; set; }
    //public string DayOfMeeting { get; set; }
    //public string Toastmaster { get; set; }
    //public string Speaker1 { get; set; }
    //public string Speaker2 { get; set; }
    //public string GeneralEvaluator { get; set; }
    //public string Evaluator1 { get; set; }
    //public string Evaluator2 { get; set; }

    //public string AhCounter { get; set; }
    //public string Grammarian { get; set; }
    //public string Timer { get; set; }
    //public string QuizMaster { get; set; }
    //public string Video { get; set; }
    //public string HotSeat { get; set; }
    //public List<string> Attendees { get; set; }

    //public string Resolved { get; set; }

    //public string WOTD { get; set; }

    //public string Theme { get; set; }
    public string TableTopics { get; set; }
    public string TTWinner { get; set; }
    private ObservableCollection<string> _ttContestants = new ObservableCollection<string>();
    public ObservableCollection<string> TTContestants { get { return _ttContestants; } set { _ttContestants = value; } }

    //public MeetingModelRegular Deserialize(string json)
    //{
    //  var options = new JsonSerializerOptions
    //  {
    //    AllowTrailingCommas = true
    //  };

    //  byte[] data = Encoding.UTF8.GetBytes(json);
    //  Utf8JsonReader reader = new Utf8JsonReader(data, isFinalBlock: true, state: default);
    //  string propertyName = string.Empty;
    //  MeetingModelBase f = null;
    //  MeetingModelRegular steve = new MeetingModelRegular();
    //  reader.Read();reader.Read();
    //  var name = reader.GetString();
    //  reader.Read();
    //  var meetingtype = reader.GetString();

    //  //writer.WriteString("MeetingType", MeetingType);
    //  //writer.WriteString("ID", ID);
    //  //writer.WriteString("DayOfMeeting", DayOfMeeting);
    //  //writer.WriteString("Toastmaster", Toastmaster);
    //  //writer.WriteString("Speaker1", Speaker1);
    //  //writer.WriteString("Speaker2", Speaker2);
    //  //writer.WriteString("GeneralEvaluator", GeneralEvaluator);
    //  //writer.WriteString("Evaluator1", Evaluator1);
    //  //writer.WriteString("Evaluator2", Evaluator2);
    //  //writer.WriteString("TableTopics", TableTopics);
    //  //writer.WriteString("AhCounter", AhCounter);
    //  //writer.WriteString("Grammarian", Grammarian);
    //  //writer.WriteString("Timer", Timer);
    //  //writer.WriteString("QuizMaster", QuizMaster);
    //  //writer.WriteString("Video", Video);
    //  //writer.WriteString("HotSeat", HotSeat);
    //  //writer.WriteString("Attendees", Attendees?.ToString());
    //  //writer.WriteString("Resolved", Resolved);
    //  if (meetingtype == "1")
    //  {
    //    f = new MeetingModelRegular();
    //  }
    //  else if (meetingtype == "2")
    //  {
    //    f = new MeetingModel3Speaker();
    //  }
    //  else if (meetingtype == "3")
    //  {
    //    f = new MeetingModel5Speaker();
    //  }

    //  while (reader.Read())
    //  {

    //    switch(reader.TokenType)
    //    {
    //      case JsonTokenType.PropertyName:
    //        {
    //          propertyName = reader.GetString();
    //          break;
    //        }
    //      case JsonTokenType.String:
    //        {
    //          string value = reader.GetString();
    //          if (propertyName == "MeetingType")
    //          {
    //            steve.MeetingType = value;
    //          }


    //          break;

    //        }

    //    }

    //  }
    //  return steve;
    //  //return JsonSerializer.Parse<MeetingModelRegular>(json, options);

    //}
    public string Serialize(MeetingModelBase meeting)
    {
      //var options = new JsonWriterOptions
      //{
      //  Indented = true
      //};

      //using (var stream = new System.IO.MemoryStream())
      //{
      //  using (var writer = new System.Text.Json.Utf8JsonWriter(stream))//, options))
      //  {
      //    writer.WriteStartObject();
      //    writer.WriteString("MeetingType", MeetingType);
      //    writer.WriteString("ID", ID);
      //    writer.WriteString("DayOfMeeting", DayOfMeeting);
      //    writer.WriteString("Toastmaster", Toastmaster);
      //    writer.WriteString("Speaker1", Speaker1);
      //    writer.WriteString("Speaker2", Speaker2);
      //    writer.WriteString("GeneralEvaluator", GeneralEvaluator);
      //    writer.WriteString("Evaluator1", Evaluator1);
      //    writer.WriteString("Evaluator2", Evaluator2);
      //    writer.WriteString("TableTopics", TableTopics);
      //    writer.WriteString("AhCounter", AhCounter);
      //    writer.WriteString("Grammarian", Grammarian);
      //    writer.WriteString("Timer", Timer);
      //    writer.WriteString("QuizMaster", QuizMaster);
      //    writer.WriteString("Video", Video);
      //    writer.WriteString("HotSeat", HotSeat);
      //    writer.WriteString("Attendees", Attendees?.ToString());
      //    writer.WriteString("Resolved", Resolved);
      //    writer.WriteEndObject();
      //  }

      //  return Encoding.UTF8.GetString(stream.ToArray());
      //}
      return "";
      // return JsonSerializer.ToString<MeetingModelRegular>(value, options);
    }

    public void Serialize(Utf8JsonWriter writer)
    {
      writer.WriteString("Speaker1", Speaker1);
      writer.WriteString("Speaker2", Speaker2);
      writer.WriteString("GeneralEvaluator", GeneralEvaluator);
      writer.WriteString("Evaluator1", Evaluator1);
      writer.WriteString("Evaluator2", Evaluator2);
      writer.WriteString("TableTopics", TableTopics);
      writer.WriteString("AhCounter", AhCounter);
      writer.WriteString("Grammarian", Grammarian);
      writer.WriteString("Timer", Timer);
      writer.WriteString("QuizMaster", QuizMaster);
      writer.WriteString("Video", Video);
      writer.WriteString("HotSeat", HotSeat);
      writer.WriteString("TTWinner", TTWinner);
      writer.WriteStartArray("TTContestants");
      foreach (string m in TTContestants)
      {
        writer.WriteStringValue(m);
      }
      writer.WriteEndArray();
      return;
    }
    public string Serialize(MeetingModelRegular meeting)
    {

      string theMeeting = string.Empty;
      return theMeeting;
    }
    public MeetingModelRegular Deserialize(string json)
    {
      var options = new JsonSerializerOptions
      {
        AllowTrailingCommas = true
      };

      byte[] data = Encoding.UTF8.GetBytes(json);
      Utf8JsonReader reader = new Utf8JsonReader(data, isFinalBlock: true, state: default);
      string propertyName = string.Empty;
      MeetingModelRegular f = null;
      //MeetingModelRegular steve = new MeetingModelRegular();
      reader.Read(); reader.Read();
      var name = reader.GetString();
      reader.Read();
      var meetingtype = reader.GetString();

      if (meetingtype == "1")
        f = new MeetingModelRegular();
      //else if (meetingtype == "4")
      //  f = new MeetingModel4Speaker();
      //else if (meetingtype == "30" || meetingtype == "20" || meetingtype == "10")
      //  f = new MeetingContest();

      while (reader.Read())
      {

        switch (reader.TokenType)
        {
          case JsonTokenType.PropertyName:
            {
              propertyName = reader.GetString();
              break;
            }
          case JsonTokenType.Number:
            {
              int value = reader.GetInt32();
              if (propertyName == "ID")
              {
                f.ID = value;
                break;
              }
              break;
            }
          case JsonTokenType.String:
            {
              string value = reader.GetString();

              if (propertyName == "DayOfMeeting")
                f.DayOfMeeting = value;

              if (propertyName == "Toastmaster")
                f.Toastmaster = value;

              if (propertyName == "Speaker1")
                f.Speaker1 = value;

              if (propertyName == "Speaker2")
                f.Speaker2 = value;

              if (propertyName == "GeneralEvaluator")
                f.GeneralEvaluator = value;

              if (propertyName == "Evaluator1")
                f.Evaluator1 = value;

              if (propertyName == "Evaluator2")
                f.Evaluator2 = value;

              if (propertyName == "TableTopics")
                (f as MeetingModelRegular).TableTopics = value;

              if (propertyName == "AhCounter")
                f.AhCounter = value;

              if (propertyName == "Grammarian")
                f.Grammarian = value;

              if (propertyName == "Timer")
                f.Timer = value;

              if (propertyName == "QuizMaster")
                f.QuizMaster = value;

              if (propertyName == "Video")
                f.Video = value;

              if (propertyName == "HotSeat")
                f.HotSeat = value;

              if (propertyName == "WOTD")
                f.WOTD = value;

              if (propertyName == "Theme")
                f.Theme = value;

              if (propertyName == "TTWinner")
                f.TTWinner = value;

              break;

            }
          case JsonTokenType.True:
          case JsonTokenType.False:
            {
              if (propertyName == "Resolved")
              {
                bool value = reader.GetBoolean();
                f.GetType().GetProperty(propertyName).SetValue(f, value, null);
                f.Resolved = value;
                break;
              }
              break;
            }
          case JsonTokenType.StartArray:
            {
              if (propertyName == "Attendees")
              {
                reader.Read();
                f.Attendees = new ObservableCollection<string>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  f.Attendees.Add(reader.GetString());
                  reader.Read();
                }

                break;
              }
              else if (propertyName == "TTContestants")
              {
                reader.Read();
                f.TTContestants = new ObservableCollection<string>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  f.TTContestants.Add(reader.GetString());
                  reader.Read();
                }

                break;
              }
              else if (propertyName == "Guests")
              {
                reader.Read();
                f.Guests = new ObservableCollection<string>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  f.Guests.Add(reader.GetString());
                  reader.Read();
                }

                break;
              }
              break;
            }

        }

      }
      return f;
      //return JsonSerializer.Parse<MeetingModelRegular>(json, options);

    }

    public void Generate(DateTime dayofmeeting, ObservableCollection<MemberViewModel> members)
    {

      var iterationMembers = members.Where(it => it.MeetingsOut.Contains(dayofmeeting)).ToList();
      foreach (var im in iterationMembers)
        members.Remove(im);
      Speaker1 = members.OrderBy(a => a.Speaker).First().ToString();

    }
  }

  public class MeetingModel3Speaker : MeetingModelBase
  {
    public string Speaker3 { get; set; }
    public string Evaluator3 { get; set; }

    public string Serialize(MeetingModelBase meeting)
    {
      //var options = new JsonWriterOptions
      //{
      //  Indented = true
      //};

      using (var stream = new System.IO.MemoryStream())
      {
        using (var writer = new System.Text.Json.Utf8JsonWriter(stream))//, options))
        {
          writer.WriteStartObject();
          writer.WriteNumber("MeetingType", MeetingType);
          writer.WriteNumber("ID", ID);
          writer.WriteString("DayOfMeeting", DayOfMeeting);
          writer.WriteString("Toastmaster", Toastmaster);
          writer.WriteString("Speaker1", Speaker1);
          writer.WriteString("Speaker2", Speaker2);
          writer.WriteString("GeneralEvaluator", GeneralEvaluator);
          writer.WriteString("Evaluator1", Evaluator1);
          writer.WriteString("Evaluator2", Evaluator2);
          writer.WriteString("AhCounter", AhCounter);
          writer.WriteString("Grammarian", Grammarian);
          writer.WriteString("Timer", Timer);
          writer.WriteString("QuizMaster", QuizMaster);
          writer.WriteString("Video", Video);
          writer.WriteString("HotSeat", HotSeat);
          writer.WriteString("Attendees", Attendees?.ToString());
          writer.WriteBoolean("Resolved", Resolved);
          writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
      }

      // return JsonSerializer.ToString<MeetingModelRegular>(value, options);
    }
  }

  public class MeetingModel4Speaker : MeetingModel3Speaker
  {
    public MeetingModel4Speaker()
    { }
    public string Speaker4 { get; set; }
    public string Evaluator4 { get; set; }

    public string Serialize(MeetingModelBase meeting)
    {
      //var options = new JsonWriterOptions
      //{
      //  Indented = true
      //};

      using (var stream = new System.IO.MemoryStream())
      {
        using (var writer = new System.Text.Json.Utf8JsonWriter(stream))//, options))
        {
          writer.WriteStartObject();
          writer.WriteNumber("MeetingType", MeetingType);
          writer.WriteNumber("ID", ID);
          writer.WriteString("DayOfMeeting", DayOfMeeting);
          writer.WriteString("Toastmaster", Toastmaster);
          writer.WriteString("Speaker1", Speaker1);
          writer.WriteString("Speaker2", Speaker2);
          writer.WriteString("GeneralEvaluator", GeneralEvaluator);
          writer.WriteString("Evaluator1", Evaluator1);
          writer.WriteString("Evaluator2", Evaluator2);
          writer.WriteString("AhCounter", AhCounter);
          writer.WriteString("Grammarian", Grammarian);
          writer.WriteString("Timer", Timer);
          writer.WriteString("QuizMaster", QuizMaster);
          writer.WriteString("Video", Video);
          writer.WriteString("HotSeat", HotSeat);
          writer.WriteString("Attendees", Attendees?.ToString());
          writer.WriteBoolean("Resolved", Resolved);
          writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
      }

      // return JsonSerializer.ToString<MeetingModelRegular>(value, options);
    }

  }
  public class MeetingModel5Speaker : MeetingModel4Speaker
  {
    public MeetingModel5Speaker()
    { }

    public string Speaker5 { get; set; }
    public string Evaluator5 { get; set; }
  }
  //public class MeetingModel
  //{
  //  public int ID { get; set; }
  //  public DateTime DayOfMeeting { get; set; }
  //  public MemberModel Toastmaster { get; set; }
  //  public MemberModel Speaker1 { get; set; }
  //  public MemberModel Speaker2 { get; set; }
  //  public MemberModel GeneralEvaluator { get; set; }
  //  public MemberModel Evaluator1 { get; set; }
  //  public MemberModel Evaluator2 { get; set; }
  //  public MemberModel TT { get; set; }
  //  public MemberModel Ah { get; set; }
  //  public MemberModel Gram { get; set; }
  //  public MemberModel Timer { get; set; }
  //  public MemberModel Quiz { get; set; }
  //  public MemberModel Video { get; set; }
  //  public MemberModel HotSeat { get; set; }
  //  public List<int> Attendees { get; set; }
  //  public MemberModel TTWinner { get; set; }
  //  public List<int> TTContestants { get; set; }
  //  public bool Resolved { get; set; }
  //  public int MeetingType { get; set; }
  //  public MeetingModel()
  //  {

  //  }

  //  public string DayOfMeetingS
  //  {
  //    get { return DayOfMeeting.ToString(); }
  //  }


  //  public MeetingModel(string[] record, ref ObservableCollection<MemberModel> members)
  //  {
  //    if (!string.IsNullOrEmpty(record[0])) ID = Int32.Parse(record[0]);
  //    if (!string.IsNullOrEmpty(record[1])) DayOfMeeting = DateTime.Parse(record[1]);
  //    if (!string.IsNullOrEmpty(record[2])) Toastmaster = members.Where(it => it.MemberID == Int32.Parse(record[2])).FirstOrDefault();


  //    if (!string.IsNullOrEmpty(record[3])) Speaker1 = members.Where(it => it.MemberID == Int32.Parse(record[3])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[4])) Speaker2 = members.Where(it => it.MemberID == Int32.Parse(record[4])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[5])) GeneralEvaluator = members.Where(it => it.MemberID == Int32.Parse(record[5])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[6])) Evaluator1 = members.Where(it => it.MemberID == Int32.Parse(record[6])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[7])) Evaluator2 = members.Where(it => it.MemberID == Int32.Parse(record[7])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[8])) TT = members.Where(it => it.MemberID == Int32.Parse(record[8])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[9])) Ah = members.Where(it => it.MemberID == Int32.Parse(record[9])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[10])) Gram = members.Where(it => it.MemberID == Int32.Parse(record[10])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[11])) Timer = members.Where(it => it.MemberID == Int32.Parse(record[11])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[12])) Quiz = members.Where(it => it.MemberID == Int32.Parse(record[12])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[13])) Video = members.Where(it => it.MemberID == Int32.Parse(record[13])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[14])) HotSeat = members.Where(it => it.MemberID == Int32.Parse(record[14])).FirstOrDefault();
  //    if (!string.IsNullOrEmpty(record[15]))
  //    {
  //      string m = record[15];
  //      char[] delims = new char[] { ';' };
  //      List<string> t = m.Split(delims, StringSplitOptions.None).ToList();
  //      Attendees = t.Select(it => Int32.Parse(it)).ToList();
  //    }
  //    if (!string.IsNullOrEmpty(record[16])) TTWinner = members.Where(it => it.MemberID == Int32.Parse(record[16])).FirstOrDefault();

  //    if (!string.IsNullOrEmpty(record[17]))
  //    {
  //      string m = record[17];
  //      char[] delims = new char[] { ';' };
  //      List<string> t = m.Split(delims, StringSplitOptions.None).ToList();
  //      TTContestants = t.Select(it => Int32.Parse(it)).ToList();
  //    }

  //    if (!string.IsNullOrEmpty(record[18]))
  //      Resolved = Int32.Parse(record[18]) == 0 ? false : true;

  //    if (!string.IsNullOrEmpty(record[19]))
  //      MeetingType = Int32.Parse(record[19]);
  //  }

  //  string Serialize(MeetingModel value)
  //  {
  //    var options = new JsonSerializerOptions
  //    {
  //      WriteIndented = true
  //    };
  //    return JsonSerializer.ToString<MeetingModel>(value, options);
  //  }
  //  public void Save(int meetingID)
  //  {
  //    // save meeting to file, append to existing file
  //    //System.IO.FileStream fileStream = new FileStream("C:\\Users\\mike\\Documents\\TI\\Meetings.dat", FileMode.Append, FileAccess.Write);
  //    //StreamWriter strmWriter = new StreamWriter("C:\\Users\\mike\\Documents\\TI\\Meetings.dat", true);
  //    //strmWriter.WriteLine(ToFile(meetingID));
  //    //strmWriter.Close();
  //    var json = JsonSerializer.ToString<MeetingModel>(this);
  //    //StreamWriter strmWriter = new StreamWriter("C:\\Users\\mike\\Documents\\TI\\Meetings.json", true);
  //    //strmWriter.WriteLine(json);
  //    //strmWriter.Close();
  //    //string firstLine = strmReader.ReadLine();
  //    //string line;
  //    //char[] delims = new char[] { ',' };
  //    //List<MeetingModel> theList = new List<MeetingModel>();
  //    //while ((line = strmReader.ReadLine()) != null)
  //    //{
  //    //  string[] pole = line.Split(delims, StringSplitOptions.None);
  //    //  MeetingModel rcd = new MeetingModel(pole, ref _members);

  //    //  theList.Add(rcd);
  //    //}

  //    //theList.Sort((x, y) => DateTime.Compare(x.DayOfMeeting, y.DayOfMeeting));
  //    //theList.Reverse();
  //    //_meetings = new ObservableCollection<MeetingModel>(theList);
  //    // Resolve meeting in file? How to write to middle of file.
  //    //return JsonSerializer.ToString();
  //    // persist info for the meeting to file meeting.dat?
  //    //SqlConnection sqlConn = new SqlConnection();
  //    //SqlCommand sqlComm = new SqlCommand();
  //    //sqlConn.ConnectionString = @"Server=.\SQLEXPRESS;Database=Scheduler;Integrated Security=true;";
  //    //sqlConn.Open();
  //    //sqlComm = sqlConn.CreateCommand();
  //    //string strTM = "UPDATE Testmembers SET Toastmaster=@Toastmaster WHERE MemberID=@MemberID";
  //    //sqlComm.CommandText = strTM;
  //    ////DateTime thedate = new DateTime(2018, 5, 18);
  //    //sqlComm.Parameters.AddWithValue("@Toastmaster", DayOfMeeting);
  //    //sqlComm.Parameters.AddWithValue("@MemberID", Toastmaster.MemberID);
  //    //sqlComm.ExecuteNonQuery();

  //    //SqlCommand spkr = new SqlCommand();
  //    //spkr = sqlConn.CreateCommand();
  //    //spkr.Parameters.AddWithValue("@Speaker", DayOfMeeting);
  //    //spkr.Parameters.AddWithValue("@MemberID", Speaker1.MemberID);
  //    //string queryspkr = "UPDATE Testmembers SET Speaker=@Speaker WHERE MemberID=@MemberID";
  //    //spkr.CommandText = queryspkr;
  //    //spkr.ExecuteNonQuery();

  //    //SqlCommand eval = new SqlCommand();
  //    //eval = sqlConn.CreateCommand();
  //    //eval.Parameters.AddWithValue("@Evaluator1", DayOfMeeting);
  //    //eval.Parameters.AddWithValue("@MemberID", Evaluator1.MemberID);
  //    //string evalstr = "UPDATE Testmembers SET Evaluator1=@Evaluator1 WHERE MemberID=@MemberID";
  //    //eval.CommandText = evalstr;
  //    //eval.ExecuteNonQuery();

  //    //sqlConn.Close();
  //  }

  //  public string ToFile(int meetingID)
  //  {
  //    StringBuilder s = new StringBuilder(meetingID.ToString());
  //    s.Append(",");
  //    s.Append(DayOfMeeting.ToString("MM/dd/yyyy"));
  //    s.Append(",");
  //    s.Append(Toastmaster.MemberID.ToString());
  //    s.Append(",");

  //    s.Append(Speaker1.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Speaker2.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(GeneralEvaluator.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Evaluator1.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Evaluator2.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(TT.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Ah.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Gram.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Timer.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Quiz.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(Video.MemberID.ToString());
  //    s.Append(",");
  //    s.Append(HotSeat.MemberID.ToString());

  //    //if (!string.IsNullOrEmpty(record[15]))
  //    //{
  //    //  string m = record[15];
  //    //  char[] delims = new char[] { ';' };
  //    //  List<string> t = m.Split(delims, StringSplitOptions.None).ToList();
  //    //  Attendees = t.Select(it => Int32.Parse(it)).ToList();
  //    //}
  //    //if (!string.IsNullOrEmpty(record[16])) TTWinner = members.Where(it => it.MemberID == Int32.Parse(record[16])).FirstOrDefault();

  //    //if (!string.IsNullOrEmpty(record[17]))
  //    //{
  //    //  string m = record[17];
  //    //  char[] delims = new char[] { ';' };
  //    //  List<string> t = m.Split(delims, StringSplitOptions.None).ToList();
  //    //  TTContestants = t.Select(it => Int32.Parse(it)).ToList();
  //    //}

  //    //if (!string.IsNullOrEmpty(record[18]))
  //    //  Resolved = Int32.Parse(record[18]) == 0 ? false : true;

  //    //if (!string.IsNullOrEmpty(record[19]))
  //    //  MeetingType = Int32.Parse(record[19]);


  //    return s.ToString();

  //  }
  //  public void GenerateMeeting(List<MemberModel> members)
  //  {

  //    Speaker1 = members.OrderBy(a => a.Speaker).First();
  //    members.Remove(Speaker1);
  //    Speaker2 = members.OrderBy(a => a.Speaker).First();
  //    members.Remove(Speaker2);
  //    Evaluator1 = members.Where(a => a.CanBeEvaluator == true).OrderBy(a => a.Evaluator).First();
  //    members.Remove(Evaluator1);
  //    Evaluator2 = members.Where(a => a.CanBeEvaluator == true).OrderBy(a => a.Evaluator).First();
  //    members.Remove(Evaluator2);
  //    Toastmaster = members.Where(a => a.CanBeToastmaster == true).OrderBy(a => a.Toastmaster).First();
  //    members.Remove(Toastmaster);
  //    GeneralEvaluator = members.Where(a => a.CanBeEvaluator == true).OrderBy(a => a.GeneralEvaluator).First();
  //    members.Remove(GeneralEvaluator);
  //    TT = members.OrderBy(a => a.TT).First();
  //    members.Remove(TT);
  //    HotSeat = members.OrderBy(a => a.HotSeat).First();
  //    members.Remove(HotSeat);
  //    Gram = members.OrderBy(a => a.Gram).First();
  //    members.Remove(Gram);
  //    Ah = members.OrderBy(a => a.Ah).First();
  //    members.Remove(Ah);
  //    Quiz = members.OrderBy(a => a.Quiz).First();
  //    members.Remove(Quiz);
  //    Timer = members.OrderBy(a => a.Timer).First();
  //    members.Remove(Timer);
  //    Video = members.OrderBy(a => a.Video).First();
  //    members.Remove(Video);
  //  }

  //  public void Generate(string role, DateTime thedate)
  //  {
  //    SqlConnection sqlConn = new SqlConnection();
  //    SqlCommand sql2 = new SqlCommand();
  //    sql2 = sqlConn.CreateCommand();
  //    sql2.Parameters.AddWithValue("@Speaker", thedate);
  //    sql2.Parameters.AddWithValue("@MemberID", Speaker1.MemberID);
  //    string querystr = "UPDATE Testmembers SET Speaker=@Speaker WHERE MemberID=@MemberID";
  //    sql2.CommandText = querystr;
  //  }

  //}

  //public class MeetingModel3 : MeetingModel
  //{
  //  public MeetingModel3()
  //  { }

  //  public MemberModel Speaker3 { get; set; }
  //  public MemberModel Evaluator3 { get; set; }
  //}


}
