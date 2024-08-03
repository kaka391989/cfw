import { DatePicker } from "antd";
import vi from "antd/es/date-picker/locale/vi_VN";
import dayjs from "dayjs";

const buddhistLocale: typeof vi = {
  ...vi,
  lang: {
    ...vi.lang,
    fieldDateFormat: "DD/MM/YYYY",
    fieldDateTimeFormat: "DD/MM/YYYY HH:mm",
    yearFormat: "YYYY",
    cellYearFormat: "YYYY",
  },
};

export const LocaleDatePicker = (props: any) => {
  const { value } = props;
  const dayJsValue = dayjs(value);

  return (
    <DatePicker
      locale={buddhistLocale}
      style={{ width: "100%" }}
      value={dayJsValue}
    />
  );
};
