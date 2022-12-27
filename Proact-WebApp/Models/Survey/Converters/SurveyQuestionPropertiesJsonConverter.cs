using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Proact_WebApp.Models {
    public class SurveyQuestionPropertiesJsonConverter : JsonConverter<ISurveysQuestionModelProperties> {
       
        public override ISurveysQuestionModelProperties ReadJson(
            JsonReader reader,
            Type objectType,
            ISurveysQuestionModelProperties existingValue,
            bool hasExistingValue,
            JsonSerializer serializer ) {

            try {
                JObject jo = JObject.Load( reader );
                int typeAsInt = jo.GetValue( "type" ).Value<int>();
                SurveyQuestionType type = ( SurveyQuestionType )typeAsInt;

                switch ( type ) {
                    case SurveyQuestionType.Rating:
                    return JsonConvert.DeserializeObject<SurveysRatingQuestionModelProperties>( jo.ToString() );

                    default:
                    return JsonConvert.DeserializeObject<SurveysBaseQuestionModelProperties>( jo.ToString() );

                }
            }
            catch {
                return new SurveysBaseQuestionModelProperties();
            }
        }

     
        public override void WriteJson(
            JsonWriter writer,
            ISurveysQuestionModelProperties value,
            Newtonsoft.Json.JsonSerializer serializer ) {
            //throw new NotImplementedException();
        }
    }
}
