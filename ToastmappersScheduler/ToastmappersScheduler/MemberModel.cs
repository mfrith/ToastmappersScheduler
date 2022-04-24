using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
//using Newtonsoft.Json;

namespace Toastmappers
{
  [Serializable]

  public class MemberModel : IEquatable<MemberModel>, IComparable<MemberModel>
  {
  //  		"MemberID": 7831561 ,
		//"Name": "Jenifer Velazco-Lopez",
		//"Toastmaster": "0001-01-01",
		//"Speaker": "0001-01-01",
		//"GeneralEvaluator": "0001-01-01",
		//"Evaluator": "0001-01-01",
		//"TT": "0001-01-01",
		//"Ah": "0001-01-01",
		//"Gram": "0001-01-01",
		//"Timer": "2020-11-18",
		//"Quiz": "0001-01-01",
		//"Video": "0001-01-01",
		//"HotSeat": "0001-01-01",
		//"HasBeenOfficer": false,
		//"CanBeToastmaster": false,
		//"CanBeEvaluator": false,
		//"MeetingsOut": [],
		//"Mentors": [],
		//"IsCurrentMember": true

    public int MemberID { get; set; }
    public string Name { get; set; }

    private DateTime _toastmaster;
    public DateTime Toastmaster
    {    
        get { return _toastmaster; }
        set { if (value == _toastmaster)
          return;
        else _toastmaster = value; }
      
    }
    public DateTime Speaker { get; set; }
    public DateTime GeneralEvaluator { get; set; }
    public DateTime Evaluator { get; set; }
    public DateTime TT { get; set; }
    public DateTime Ah { get; set; }
    public DateTime Gram { get; set; }
    public DateTime Timer { get; set; }
    public DateTime Quiz { get; set; }
    public DateTime Video { get; set; }
    public DateTime HotSeat { get; set; }
    public bool HasBeenOfficer { get; set; }
    public bool CanBeToastmaster { get; set; }
    public bool CanBeEvaluator { get; set; }
    public List<DateTime> MeetingsOut { get; set; }
    public ObservableCollection<string> Mentors { get; set; }
    public bool IsCurrentMember { get; set; }
    public MemberModel()
    {

    }

    public MemberModel Deserialize(string json)
    {
      var options = new JsonSerializerOptions
      {
        AllowTrailingCommas = true
      };

      byte[] data = Encoding.UTF8.GetBytes(json);
      Utf8JsonReader reader = new Utf8JsonReader(data, isFinalBlock: true, state: default);
      string propertyName = string.Empty;
      MemberModel member = new MemberModel();
      //MemberModel member = JsonConvert.DeserializeObject<MemberModel>(json);
      //reader.Read(); reader.Read();
      //var name = reader.GetString();
      //var a = reader.TokenType;
      //reader.Read();
      //var memberID = reader.GetUInt32();

      while (reader.Read())
      {
        switch (reader.TokenType)
        {
          case JsonTokenType.PropertyName:
            {
              propertyName = reader.GetString();
              break;
            }
          case JsonTokenType.String:
            {
              string value = reader.GetString();

              if (propertyName == "Name")
              {
                member.Name = value;
                break;
              }

              if (propertyName == "Toastmaster" || propertyName == "Speaker" || propertyName == "Evaluator" || propertyName == "GeneralEvaluator" ||
                  propertyName == "TT" || propertyName == "Ah" || propertyName == "Gram" || propertyName == "Timer" || propertyName == "Quiz" ||
                  propertyName == "Video" || propertyName == "HotSeat")
              {
                var date = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                member.GetType().GetProperty(propertyName).SetValue(member, date, null);
                break;

              }
              break;
            }
          case JsonTokenType.Number:
            {
              int value = reader.GetInt32();
              if (propertyName == "MemberID")
              {
                member.MemberID = value;
                break;
              }
              break;
            }
          case JsonTokenType.True:
          case JsonTokenType.False:
            {
              if (propertyName == "CanBeEvaluator" || propertyName == "CanBeToastmaster" || propertyName == "HasBeenOfficer" || propertyName == "IsCurrentMember")
              {
                bool value = reader.GetBoolean();
                member.GetType().GetProperty(propertyName).SetValue(member, value, null);
                break;
              }
              break;
            }
          case JsonTokenType.StartArray:
            {
              if (propertyName == "MeetingsOut")
              {
                reader.Read();
                member.MeetingsOut = new List<DateTime>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  member.MeetingsOut.Add(reader.GetDateTime());
                  reader.Read();
                }

                break;
              }
              else if (propertyName == "Mentors")
              {
                reader.Read();
                member.Mentors = new ObservableCollection<string>();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                  member.Mentors.Add(reader.GetString());
                  reader.Read();
                }

                break;
              }
              break;
            }
        }

      }

      //if (member.Name == "Lisa Winn" || member.Name == "Mani Vijayakumar")
      //  member.IsCurrentMember = false;
      //else
      //  member.IsCurrentMember = true;
      return member;

    }
    public string Serialize(MemberModel member)
    {
      string strMember = string.Empty;

      //var options = new JsonWriterOptions
      //{
      //  Indented = true
      //};

      using (var stream = new System.IO.MemoryStream())
      {
        using (var writer = new Utf8JsonWriter(stream))//, options))
        {
          writer.WriteStartObject();
          writer.WriteString("Name", member.Name);
          writer.WriteNumber("MemberID", member.MemberID);
          writer.WriteString("Toastmaster", member.Toastmaster.ToString("yyyy-MM-dd"));
          writer.WriteString("Speaker", member.Speaker.ToString("yyyy-MM-dd"));
          writer.WriteString("GeneralEvaluator", member.GeneralEvaluator.ToString("yyyy-MM-dd"));
          writer.WriteString("Evaluator", member.Evaluator.ToString("yyyy-MM-dd"));
          writer.WriteString("TT", member.TT.ToString("yyyy-MM-dd"));
          writer.WriteString("Ah", member.Ah.ToString("yyyy-MM-dd"));
          writer.WriteString("Gram", member.Gram.ToString("yyyy-MM-dd"));
          writer.WriteString("Timer", member.Timer.ToString("yyyy-MM-dd"));
          writer.WriteString("Quiz", member.Quiz.ToString("yyyy-MM-dd"));
          writer.WriteString("Video", member.Video.ToString("yyyy-MM-dd"));
          writer.WriteString("HotSeat", member.HotSeat.ToString("yyyy-MM-dd"));
          writer.WriteBoolean("HasBeenOfficer", member.HasBeenOfficer);
          writer.WriteBoolean("CanBeToastmaster", member.CanBeToastmaster);
          writer.WriteBoolean("CanBeEvaluator", member.CanBeEvaluator);

            writer.WriteStartArray("MeetingsOut");
            foreach (DateTime m in member.MeetingsOut)
            {
              writer.WriteStringValue(m.ToString("yyyy-MM-dd"));
            }
            writer.WriteEndArray();


            writer.WriteStartArray("Mentors");
            foreach (string m in member.Mentors)
            {
              writer.WriteStringValue(m);
            }
            writer.WriteEndArray();
 

          writer.WriteBoolean("IsCurrentMember", member.IsCurrentMember);

          writer.WriteEndObject();

        }
        strMember = Encoding.UTF8.GetString(stream.ToArray());
      }
      return strMember;
    }

    public bool Equals(MemberModel other)
    {
      if (other == null) return false;
      return (this.Name.Equals(other.Name));
    }

    public int CompareTo(MemberModel other)
    {
      if (other == null)
        return 1;
      else
        return this.Name.CompareTo(other.Name);
    }

    public MemberModel(string[] record)
    {
      MemberID = System.Int32.Parse(record[1]);
      Name = record[2];
      if (!string.IsNullOrEmpty(record[3]))
        Toastmaster = System.DateTime.Parse(record[3]);
      if (!string.IsNullOrEmpty(record[4])) Speaker = System.DateTime.Parse(record[4]);
      if (!string.IsNullOrEmpty(record[5]))
        GeneralEvaluator = System.DateTime.Parse(record[5]);
      if (!string.IsNullOrEmpty(record[6])) Evaluator = System.DateTime.Parse(record[6]);
      if (!string.IsNullOrEmpty(record[7])) TT = System.DateTime.Parse(record[7]);
      if (!string.IsNullOrEmpty(record[8])) Ah = System.DateTime.Parse(record[8]);
      if (!string.IsNullOrEmpty(record[9])) Gram = System.DateTime.Parse(record[9]);
      if (!string.IsNullOrEmpty(record[10])) Timer = System.DateTime.Parse(record[10]);
      if (!string.IsNullOrEmpty(record[11])) Quiz = System.DateTime.Parse(record[11]);
      if (!string.IsNullOrEmpty(record[12])) Video = System.DateTime.Parse(record[12]);
      if (!string.IsNullOrEmpty(record[13])) HotSeat = System.DateTime.Parse(record[13]);
      if (!string.IsNullOrEmpty(record[14]))
        HasBeenOfficer = System.Boolean.Parse(record[14]);
      else
        HasBeenOfficer = false;


      if (!string.IsNullOrEmpty(record[15]))
        CanBeToastmaster = System.Boolean.Parse(record[15]);
      else
        CanBeToastmaster = false;

      if (!string.IsNullOrEmpty(record[16]))
        CanBeEvaluator = System.Boolean.Parse(record[16]);
      else
        CanBeEvaluator = false;

      if (!string.IsNullOrWhiteSpace(record[17]))
      {
        string m = record[17];
        char[] delims = new char[] { ';' };
        List<string> t = m.Split(delims, StringSplitOptions.None).ToList();
        //MeetingsOut = t.Select(it => System.Int32.Parse(it)).ToList();
      }

      if (!string.IsNullOrEmpty(record[18]))
      {
        string m = record[18];
        char[] delims = new char[] { ';' };
        var c = m.Split(delims, StringSplitOptions.None).ToList();
        Mentors = new ObservableCollection<string>(c);

      }
    }

  }
}

