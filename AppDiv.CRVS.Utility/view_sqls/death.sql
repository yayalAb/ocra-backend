-- #death10 1 by during death 
SELECT 
	a.*,
    EventRegDate ,
    l.ValueStr AS DuringDeath,
    COUNT(d.Id) AS Total
FROM DeathEvents AS d
JOIN Events as e ON e.Id = d.EventId
JOIN Lookups AS l ON l.Id = DuringDeathId
JOIN view_address as a on a.addId = e.EventRegisteredAddressId
GROUP BY l.Id,EventRegisteredAddressId,EventRegDate;
-- #death10 2 by region
SELECT 
	ifNull(a.Region,a.Country) as Address,
    EventRegDate ,
    COUNT(d.Id) AS Total
FROM DeathEvents AS d
JOIN Events as e ON e.Id = d.EventId
JOIN view_address as a on a.addId = e.EventAddressId
GROUP BY ifnull(RegId,conid),EventRegDate;
-- #death10 3 by gender
SELECT 
	a.*,
    l.ValueStr,
    EventRegDate ,
    COUNT(d.Id) AS Total
FROM DeathEvents AS d
JOIN Events as e ON e.Id = d.EventId
JOIN PersonalInfos as p ON p.Id = e.EventOwenerId
JOIN Lookups as l ON l.Id = p.SexLookupId
JOIN view_address as a on a.addId = e.EventRegisteredAddressId
GROUP BY l.Id,EventRegisteredAddressId,EventRegDate;
-- #death4 1 by desceased age range and resident address region
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
ifNull(a.Region,a.Country) as Address,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = per.ResidentAddressId
GROUP BY ren.id , ifnull(RegId,conid);
-- #death4 2 by desceased age range and gender
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
	a.*,
    l.ValueStr,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Lookups as l on l.Id = per.SexLookupId
GROUP BY ren.id ,a.addId, l.Id;


-- #death4 3 by desceased age range and resident address region -- male
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
ifNull(a.Region,a.Country) as Address,
    l.ValueStr as Gender,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = per.ResidentAddressId
JOIN Lookups as l on l.Id = per.SexLookupId
where l.ValueStr LIKE '%"Male%'
GROUP BY ren.id , ifnull(RegId,conid);

-- #death4 4 by desceased age range and resident address region -- female
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
ifNull(a.Region,a.Country) as Address,
    l.ValueStr as Gender,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = per.ResidentAddressId
JOIN Lookups as l on l.Id = per.SexLookupId
where l.ValueStr LIKE '%Female%'
GROUP BY ren.id , ifnull(RegId,conid);

-- #death4 5 by desceased age range and resident address region -- not stated
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
ifNull(a.Region,a.Country) as Address,
    l.ValueStr as Gender,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = per.ResidentAddressId
JOIN Lookups as l on l.Id = per.SexLookupId
where  not (l.ValueStr LIKE '%Female%' or l.ValueStr LIKE '%"Male%')
GROUP BY ren.id , ifnull(RegId,conid);

-- #death4 6 by desceased age range and gender
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
	a.*,
    l.ValueStr as Gender,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Lookups as l on l.Id = per.SexLookupId
GROUP BY ren.id ,a.addId, l.Id;
-- #death4 7 by desceased age range only
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
	a.*,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
GROUP BY ren.id , a.addId ;

-- #sheet7 1 by desceased age range and educationstatus
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
	a.*,
    l.ValueStr as education,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Lookups as l on l.Id = per.EducationalStatusLookupId
GROUP BY ren.id , a.addId, EducationalStatusLookupId;

-- #sheet7 2 by death facility
SELECT
	-- a.*,
    l.ValueStr as facility,
    ev.EventRegDate,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Lookups as l on l.Id = FacilityLookupId
GROUP BY  a.addId, FacilityLookupId;

-- #sheet6 1 by death facility and zone
SELECT
	a.zone as Zone,
    l.ValueStr as facility,
    ev.EventRegDate,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Lookups as l on l.Id = FacilityLookupId
GROUP BY  a.zoneid, FacilityLookupId;


-- #sheet6 2 by death facility and region
SELECT
	a.Region as Region,
    l.ValueStr as facility,
    ev.EventRegDate,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN Lookups as l on l.Id = FacilityLookupId
GROUP BY  a.regid, FacilityLookupId;

-- #Death1 1 by deceased gender and areatype
SELECT
    Atyp.ValueStr as areaType,
    gend.ValueStr as Gender,
    ev.EventRegDate,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventAddressId
JOIN PersonalInfos as per on per.Id = ev.EventOwenerId
JOIN Lookups as Atyp on Atyp.Id = a.AreaTypeId
JOIN Lookups as gend on gend.Id = per.SexLookupId
GROUP BY  Atyp.Id, per.SexLookupId;

-- #Death1 2 by deceased gender and event address region
SELECT
   ifnull(Region,Country) as address,
    gend.ValueStr as Gender,
    ev.EventRegDate,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventAddressId
JOIN PersonalInfos as per on per.Id = ev.EventOwenerId
JOIN Lookups as gend on gend.Id = per.SexLookupId
GROUP BY  ifnull(RegId,conid), per.SexLookupId;
-- #Sheet1 =--duplicate --with death10
-- 
-- #Death3 =--duplicate --with sheet6
-- 
-- #Death2 death registration month by event month
SELECT
    a.*,
    substring(EventRegDateEt,4,2) as EventRegisteredMonth,
    substring(EventDateEt,4,2) as EventDateMonth,
    right(EventRegDateEt,4) as EventRegYear,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
GROUP BY EventRegisteredAddressId ,EventRegisteredMonth,EventRegYear ,EventDateMonth
order by EventRegYear ,EventRegisteredMonth,EventDateMonth;

-- #Death2 death registration month by region
SELECT
    substring(EventRegDateEt,4,2) as EventRegisteredMonth,
    a.Zone as Zone,
    right(EventRegDateEt,4) as EventRegYear,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
GROUP BY a.zoneid,EventRegisteredMonth,EventRegYear
order by EventRegYear ,EventRegisteredMonth; 

-- #Death5 1 deseased age range by marriage status
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
    a.*,
    l.ValueStr as MartialStatus,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = per.ResidentAddressId
JOIN Lookups as l on l.Id = per.MarriageStatusLookupId
GROUP BY ren.id , a.addId,MarriageStatusLookupId;

-- #Death5 2 deseased age range by martialStatus female
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
-- a.*,
    edu.ValueStr as MartialStatus,
    gend.ValueStr as Gender,
    ev.EventRegDate,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = per.ResidentAddressId
JOIN Lookups as edu on edu.Id = per.MarriageStatusLookupId
JOIN Lookups as gend on gend.Id = per.SexLookupId
where gend.ValueStr LIKE '%Female%'
GROUP BY ren.id , a.addId,MarriageStatusLookupId;

-- #Death6 duplicate with sheet7
-- 
-- #Death7 registration status by reg address region
SELECT
   ifnull(Region,Country) as address,
   ev.Status as RegistrationStatus,
    ev.EventRegDate,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
GROUP BY  ifnull(regid,conid), ev.Status;
-- Basi
-- 
-- 
-- Death duplicate with death4
-- 
-- f6.2 ????

-- #report2 death registration month by gender
SELECT
	a.*,
    substring(EventRegDateEt,4,2) as EventRegisteredMonth,
    l.ValueStr as Gender,
    right(EventRegDateEt,4) as EventRegYear,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
JOIN view_address as a on a.addId = ev.EventRegisteredAddressId
JOIN PersonalInfos as per on per.Id = ev.EventOwenerId
JOIN Lookups as l on l.Id = per.SexLookupId
GROUP BY  l.Id, EventRegisteredMonth,EventRegYear, a.addid
order by EventRegYear ,EventRegisteredMonth; 

-- #report2 death registration month by age range
SELECT CONCAT(ren.Start, - ren.End) AS yearRange,
    a.*,
    substring(EventRegDateEt,4,2) as EventRegisteredMonth,
    right(EventRegDateEt,4) as EventRegYear,
    ren.Start, ren.End ,
    YEAR(ev.EventDate)- YEAR(per.BirthDate) AS Age,
    COUNT(d.Id) AS Count
FROM
    DeathEvents AS d
JOIN Events as ev ON ev.Id = d.EventId
LEFT JOIN
    PersonalInfos AS per ON per.id = ev.EventowenerId
LEFT JOIN
    SystemRanges AS ren ON ren.Key = 'ageRange' and YEAR(ev.EventDate)- YEAR(per.BirthDate) between ren.Start and ren.End
JOIN view_address as a on a.addId = per.ResidentAddressId
GROUP BY ren.id , a.addId,EventRegisteredMonth,EventRegYear
order by EventRegYear ,EventRegisteredMonth;

