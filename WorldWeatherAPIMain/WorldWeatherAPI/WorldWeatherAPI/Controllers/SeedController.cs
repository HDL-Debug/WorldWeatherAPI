﻿using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WorldWeatherAPI.Data.Models;
using WorldWeatherAPI.Data;

namespace WorldWeatherAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SeedController(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // prevents non-development environments from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");
            var path = Path.Combine(
                _env.ContentRootPath,
                "Data/Source/worldweatherReport.xlsx");
            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);
            // get the first worksheet 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var worksheet = excelPackage.Workbook.Worksheets[0];
            // define how many rows we want to process 
            var nEndRow = worksheet.Dimension.End.Row;
            // initialize the record counters 
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;
            // create a lookup dictionary 
            // into the Database (it will be empty on first run).
            var countriesByName = _context.Countries
                .AsNoTracking()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            // iterates through all rows, skipping the first one 
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                    nRow, 1, nRow, worksheet.Dimension.End.Column];
                var countryName = row[nRow, 5].GetValue<string>();
                var iso2 = row[nRow, 6].GetValue<string>();
                var iso3 = row[nRow, 7].GetValue<string>();
                // skip this country if it already exists in the database
                if (countriesByName.ContainsKey(countryName))
                    continue;
                // create the Country entity and fill it with xlsx data 
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };
                // add the new country to the DB context 
                await _context.Countries.AddAsync(country);
                // store the country in our lookup to retrieve its Id later on
                countriesByName.Add(countryName, country);
                // increment the counter 
                numberOfCountriesAdded++;
            }
            // save all the countries into the Database 
            if (numberOfCountriesAdded > 0)
                await _context.SaveChangesAsync();
            // create a lookup dictionary
            // containing all the cities already existing 
            // into the Database (it will be empty on first run). 
            var cities = _context.Cities
                .AsNoTracking()
                .ToDictionary(x => (
                    Name: x.Name,
                    Temperature: x.Temp,
                    Percipitation: x.Perci,
                    CountryId: x.CountryId));
            // iterates through all rows, skipping the first one 
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                    nRow, 1, nRow, worksheet.Dimension.End.Column];
                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var temp = row[nRow, 3].GetValue<decimal>();
                var perci = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();
                // retrieve country Id by countryName
                var countryId = countriesByName[countryName].Id;
                // skip this city if it already exists in the database
                if (cities.ContainsKey((
                    Name: name,
                    Temperature: temp,
                    Percipitation: perci,
                    CountryId: countryId)))
                    continue;
                // create the City entity and fill it with xlsx data 
                var city = new City
                {
                    Name = name,
                    Temp = temp,
                    Perci = perci,
                    CountryId = countryId
                };
                // add the new city to the DB context 
                _context.Cities.Add(city);
                // increment the counter 
                numberOfCitiesAdded++;
            }
            // save all the cities into the Database 
            if (numberOfCitiesAdded > 0)
                await _context.SaveChangesAsync();
            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            });
        }
    }
}
