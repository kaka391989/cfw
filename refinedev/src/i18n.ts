import i18n from "i18next";
import detector from "i18next-browser-languagedetector";
import Backend from "i18next-http-backend";
import { initReactI18next } from "react-i18next";

i18n
  .use(Backend)
  .use(detector)
  .use(initReactI18next)
  .init({
    supportedLngs: ["vi", "en"],
    backend: {
      loadPath: "/locales/{{lng}}/{{ns}}.json",
    },
    lng: "vi",
    ns: ["common"],
    defaultNS: "common",
    fallbackLng: ["vi", "en"],
  });

export default i18n;
