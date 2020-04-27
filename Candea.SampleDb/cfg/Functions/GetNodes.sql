CREATE FUNCTION [cfg].[GetNodes]
(
)
RETURNS TABLE AS RETURN
(
	select Id, [Name], [Value], CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
	from cfg.[Node]
)
