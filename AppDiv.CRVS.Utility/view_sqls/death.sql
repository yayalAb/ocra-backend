-- #death10 1
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
-- #deatj10 2
SELECT 
	ifNull(a.Region,a.Country) as Address,
    EventRegDate ,
    COUNT(d.Id) AS Total
FROM DeathEvents AS d
JOIN Events as e ON e.Id = d.EventId
JOIN view_address as a on a.addId = e.EventAddressId
GROUP BY ifnull(RegId,conid),EventRegDate;