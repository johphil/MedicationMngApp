using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    /// <summary>
    /// Used to store and send the user's ratings and recommendation
    /// </summary>
    public class Ratings_Recommendation
    {
        [JsonProperty(PropertyName = "Ratings_Recommendation_ID")]
        public int Ratings_Recommendation_ID { get; set; }
        [JsonProperty(PropertyName = "Account_ID")]
        public int Account_ID { get; set; }
        [JsonProperty(PropertyName = "Ratings")]
        public int Ratings { get; set; }
        [JsonProperty(PropertyName = "Recommendation")]
        public string Recommendation { get; set; }
        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }
    }

    #region API Helpers
    public class AddRatingsRecommendationRequestObject
    {
        [JsonProperty(PropertyName = nameof(ratings))]
        public Ratings_Recommendation ratings { get; set; }
    }

    public class AddRatingsRecommendationResult
    {
        [JsonProperty(PropertyName = nameof(AddRatingsRecommendationResult))]
        public int result { get; set; }
    }

    public class GetRatingsRecommendationResult
    {
        [JsonProperty(PropertyName = nameof(GetRatingsRecommendationResult))]
        public Ratings_Recommendation ratings { get; set; }
    }
    #endregion //END API Helpers
}
