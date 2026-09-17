import type { SavedMeasurementProfile } from "../types/measurements";

const storageKey = "bili-store.measurement-profiles";

export function loadMeasurementProfiles(): SavedMeasurementProfile[] {
  if (typeof window === "undefined") {
    return [];
  }

  try {
    const storedValue = window.localStorage.getItem(storageKey);
    return storedValue ? (JSON.parse(storedValue) as SavedMeasurementProfile[]) : [];
  } catch {
    return [];
  }
}

export function saveMeasurementProfiles(profiles: SavedMeasurementProfile[]) {
  if (typeof window === "undefined") {
    return;
  }

  window.localStorage.setItem(storageKey, JSON.stringify(profiles));
}

export function createMeasurementProfile(
  name: string,
  values: Record<string, string>,
): SavedMeasurementProfile {
  return {
    id: crypto.randomUUID(),
    name,
    values,
    updatedAt: new Date().toISOString(),
  };
}
