-- f3.1 Proportion (%) of live births and death by status by year
SELECT
	-- a.*,
    ev.Status as RegisterationStatus,
    right(EventRegDateEt,4) as EventRegYear,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN PersonalInfos as per on per.Id = ev.EventOwenerId
GROUP BY a.addId, RegisterationStatus,EventRegYear
order by EventRegYear; 

-- f3.1 birth and death by status by week
SELECT
	a.*,
	substring(EventRegDateEt,4,2) as EventRegisteredMonth,
    CASE
           WHEN left(EventRegDateEt,2) <= 7 THEN 'week1'
           WHEN left(EventRegDateEt,2) > 7 and left(EventRegDateEt,2) <= 14 THEN 'week2'
           WHEN left(EventRegDateEt,2) > 14 and left(EventRegDateEt,2) <= 21 THEN 'week3'
           WHEN left(EventRegDateEt,2) > 21  THEN 'week4'
           ELSE 'unknown'
       END AS eventRegWeek,
    ev.Status as RegisterationStatus,
    right(EventRegDateEt,4) as EventRegYear,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN PersonalInfos as per on per.Id = ev.EventOwenerId
GROUP BY a.addId, RegisterationStatus,EventRegYear, EventRegisteredMonth , eventRegWeek
order by EventRegYear , EventRegisteredMonth , eventRegWeek; 

-- f7.3  divorce by year population
SELECT
	a.*,
    right(EventRegDateEt,4) as EventRegYear,
    pl.PopulationSize as population,
    COUNT(div.Id) AS Count
FROM
    DivorceEvents AS div
JOIN Events as ev ON ev.Id = div.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Plans as pl on pl.AddressId = EventRegisteredAddressId and pl.BudgetYear = right(ev.EventRegDateEt,4)
GROUP BY a.addId,EventRegYear
order by EventRegYear;  

-- #marriage marriage by year with total population
SELECT
	a.*,
    right(EventRegDateEt,4) as EventRegYear,
    pl.PopulationSize as population,
    COUNT(m.Id) AS Count
FROM
    MarriageEvents AS m
JOIN Events as ev ON ev.Id = m.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Plans as pl on pl.AddressId = EventRegisteredAddressId and pl.BudgetYear = right(ev.EventRegDateEt,4)
GROUP BY a.addId,EventRegYear
order by EventRegYear;  

-- #Birth1 crude birth rate by area type
SELECT
    right(EventRegDateEt,4) as EventRegYear,
    pl.PopulationSize as population,
    l.ValueStr as AreaType,
    COUNT(b.Id) AS Count
FROM
    BirthEvents AS b
JOIN Events as ev ON ev.Id = b.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Plans as pl on pl.AddressId = EventRegisteredAddressId and  pl.BudgetYear = right(ev.EventRegDateEt,4)
JOIN Lookups as l on l.Id = a.AreaTypeId 
GROUP BY EventRegYear,l.Id
order by EventRegYear; 

-- Death duplicate -----
-- 
-- 
-- T3.10 ??
-- T3.11 - we dont have unknown data
-- F4.1 Live birth by year
SELECT
	a.*,
    right(EventRegDateEt,4) as EventRegYear,
    COUNT(b.Id) AS Count
FROM
    BirthEvents AS b
JOIN Events as ev ON ev.Id = b.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
GROUP BY a.addId,EventRegYear
order by EventRegYear; 

-- #F4.2  (1) Births by age of mother (over time)
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
	-- a.*,
    right(EventRegDateEt,4) as EventRegYear,
     ev.EventRegDate,
    COUNT(b.Id) AS Count
FROM
    BirthEvents AS b
JOIN Events as ev ON ev.Id = b.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN PersonalInfos as per on per.Id = b.MotherId
Right JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
GROUP BY a.addId,EventRegYear, ren.Id
order by EventRegYear; 
-- f4.2 (2) Births by age of mother and place of usual residence
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
	-- a.*,
    per.ResidentAddressId as motherResidentAddr,
    COUNT(b.Id) AS Count
FROM
    BirthEvents AS b
JOIN Events as ev ON ev.Id = b.EventId
JOIN PersonalInfos as per on per.Id = b.MotherId
JOIN view_address as a on a.addId = per.ResidentAddressId
Right JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
GROUP BY a.addId, ren.Id
order by yearRange; 

-- F4.4 ?? dont have population of female by age group for now
-- F5.4 ?? dont have population  by age group for now
-- F5.4 ?? dont have population  by age group for now

