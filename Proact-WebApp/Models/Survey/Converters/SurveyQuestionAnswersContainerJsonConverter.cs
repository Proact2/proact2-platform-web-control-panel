using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Proact_WebApp.Models {
    public class SurveyQuestionAnswersContainerJsonConverter : JsonConverter<ISurveyQuestionModelAnswersContainer> {

        public override ISurveyQuestionModelAnswersContainer ReadJson(
            JsonReader reader,
            Type objectType,
            ISurveyQuestionModelAnswersContainer existingValue,
            bool hasExistingValue,
            JsonSerializer serializer ) {

            try {
                JObject jo = JObject.Load( reader );
                int typeAsInt = jo.GetValue( "type" ).Value<int>();
                SurveyQuestionType type = ( SurveyQuestionType )typeAsInt;

                switch ( type ) {
                    case SurveyQuestionType.SingleChoice:
                    return JsonConvert
                        .DeserializeObject<SurveysSingleChoiceQuestionModelAnswersContainer>( jo.ToString() );

                    case SurveyQuestionType.MultipleChoice:
                    return JsonConvert
                        .DeserializeObject<SurveysMultipleChoiceQuestionModelAnswersContainer>( jo.ToString() );

                    default:
                    return JsonConvert
                       .DeserializeObject<SurveysBaseQuestionModelAnswersContainer>( jo.ToString() );

                }
            }
            catch {
                return new SurveysBaseQuestionModelAnswersContainer();
            }
        }


        public override void WriteJson(
            JsonWriter writer,
            ISurveyQuestionModelAnswersContainer value,
            JsonSerializer serializer ) {
            throw new NotImplementedException();
        }
    }
}
