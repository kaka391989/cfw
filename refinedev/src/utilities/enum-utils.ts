export const getEnumOptions = (enumObj: any) =>
  Object.keys(enumObj)
    .filter((key) => isNaN(Number(key)))
    .map((key) => ({
      label: key,
      value: key,
    }));

export const uuidEmpty = "00000000-0000-0000-0000-000000000000";
