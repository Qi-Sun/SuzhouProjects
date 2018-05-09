using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointToArea
{
    public class SQLPattern
    {

        private static string _Extract_POI_by_Superior_Title =
            @"SELECT * FROM {0}.{1} where superior_poi_title = '{2}';";

        public static string Extract_POI_by_Superior_Title(string schema, string tablename, string title)
        {
            return string.Format(_Extract_POI_by_Superior_Title, schema, tablename, title);
        }

        private static string _GetCheckWeibo = @"
SELECT 
    latitude,
    longitude,
    COUNT(POINT(latitude, longitude)) AS num
FROM
    (SELECT 
        *
    FROM
        {0}.{1}
    WHERE
        {4} IN (SELECT 
                poiid
            FROM
                {0}.{2}
            WHERE
                {5} = '{3}')) AS tmpTable
GROUP BY POINT(latitude, longitude)
ORDER BY num DESC;";

        public static string GetCheckWeibo(string schema,string checkintable,string poitable,string poiTitle,
            string annotation_place_poiid,string suporier_title)
        {
            return string.Format(_GetCheckWeibo, schema, checkintable, poitable, poiTitle, annotation_place_poiid, suporier_title);
        }

        private static string _GetPoiSuperiorTitle = @"
SELECT 
    {2} as title, COUNT({2}) AS count
FROM
    {0}.{1}
WHERE
    {2} IS NOT NULL
GROUP BY {2}
ORDER BY count DESC;";
        public static string GetPoiSuperiorTitle(string schema,string chktable,string superiorTitleName)
        {
            return string.Format(_GetPoiSuperiorTitle, schema, chktable, superiorTitleName);
        }


        public static string _Get_CityWeibos = @"
SELECT 
    *
FROM
    {0}.{1}
LIMIT {2} , {3};";

        public static string Get_CityWeibos(string schema,string table,int index, int count)
        {
            return string.Format(_Get_CityWeibos, schema, table, index, count);
        }

        public static string _UpdateTable_PredictPlace_OnlyOne = @"
UPDATE {0}.{1}
SET 
    {3} = {4},
    {5} = {6},
    {7} = {7} + 1
WHERE
    id = {2};";
        public static string Get__UpdateTable_PredictPlace_OnlyOne(string schema,string geoTable,
            string id,string place_field_name,string place, string value_field_name,string value,string marked_field)
        {
            return string.Format(_UpdateTable_PredictPlace_OnlyOne, schema, geoTable,
             id, place_field_name, place, value_field_name, value, marked_field);
        }
        public static string _Get_UserCity = @"
SELECT 
    *
FROM
    {0}.{1}
WHERE
    userid = {2};";

        public static string Get_UserCity(string schema,string table,string id)
        {
            return string.Format(_Get_UserCity, schema, table,id);
        }


    }
}
